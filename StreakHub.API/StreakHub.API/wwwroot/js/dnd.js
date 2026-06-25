window.StreakHubDnd = (() => {
  function startInput() {
    return document.getElementById('dnd-start');
  }

  function endInput() {
    return document.getElementById('dnd-end');
  }

  function statusEl() {
    return document.getElementById('dnd-status');
  }

  function formatTimeSpan(value) {
    if (!value) {
      return '';
    }

    const parts = String(value).split(':');
    return `${parts[0]?.padStart(2, '0')}:${parts[1]?.padStart(2, '0')}`;
  }

  function parseTimeInput(value) {
    if (!value) {
      return null;
    }

    return `${value}:00`;
  }

  function renderStatus(profile) {
    const status = statusEl();
    const toggle = document.getElementById('notifications-toggle');

    if (!status) {
      return;
    }

    if (profile?.dndStart && profile?.dndEnd) {
      status.textContent = `DND: ${formatTimeSpan(profile.dndStart)} – ${formatTimeSpan(profile.dndEnd)}${profile.isInDnd ? ' (đang bật)' : ''}`;
      toggle?.classList.toggle('dnd-active-indicator', profile.isInDnd);
    } else {
      status.textContent = 'DND chưa được cấu hình.';
      toggle?.classList.remove('dnd-active-indicator');
    }
  }

  function checkIsInDnd(start, end) {
    if (!start || !end) return false;
    const now = new Date();
    const currentMin = now.getHours() * 60 + now.getMinutes();

    const [startH, startM] = start.split(':').map(Number);
    const [endH, endM] = end.split(':').map(Number);

    const startMin = startH * 60 + startM;
    const endMin = endH * 60 + endM;

    if (startMin <= endMin) {
      return currentMin >= startMin && currentMin <= endMin;
    } else {
      return currentMin >= startMin || currentMin < endMin;
    }
  }

  async function load() {
    const userId = StreakHubApi.getUser()?.userId || 1;
    const res = await StreakHubApi.get(`/api/Dnd/${userId}`);
    
    const profile = {
      dndStart: res.enabled ? res.start : null,
      dndEnd: res.enabled ? res.end : null,
      isInDnd: res.enabled ? checkIsInDnd(res.start, res.end) : false
    };

    if (startInput()) {
      startInput().value = formatTimeSpan(profile.dndStart);
    }
    if (endInput()) {
      endInput().value = formatTimeSpan(profile.dndEnd);
    }
    renderStatus(profile);
    return profile;
  }

  async function save() {
    const start = startInput()?.value;
    const end = endInput()?.value;

    if (!start || !end) {
      throw new Error('Vui lòng nhập cả giờ bắt đầu và kết thúc DND.');
    }

    const userId = StreakHubApi.getUser()?.userId || 1;
    await StreakHubApi.post(`/api/Dnd/${userId}`, {
      enabled: true,
      start: start,
      end: end
    });

    await load();
  }

  async function clear() {
    const userId = StreakHubApi.getUser()?.userId || 1;
    await StreakHubApi.post(`/api/Dnd/${userId}`, {
      enabled: false,
      start: '22:00',
      end: '07:00'
    });

    if (startInput()) {
      startInput().value = '';
    }
    if (endInput()) {
      endInput().value = '';
    }

    await load();
  }

  function bindUi() {
    document.getElementById('dnd-save')?.addEventListener('click', async () => {
      try {
        await save();
      } catch (error) {
        if (statusEl()) {
          statusEl().textContent = error.message;
        }
      }
    });

    document.getElementById('dnd-clear')?.addEventListener('click', async () => {
      try {
        await clear();
      } catch (error) {
        if (statusEl()) {
          statusEl().textContent = error.message;
        }
      }
    });
  }

  bindUi();

  return {
    load,
    save,
    clear,
    renderStatus
  };
})();

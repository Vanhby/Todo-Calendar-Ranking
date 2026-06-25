window.StreakHubApi = (() => {
  const TOKEN_KEY = 'streakhub_token';
  const USER_KEY = 'streakhub_user';

  function getToken() {
    return localStorage.getItem(TOKEN_KEY);
  }

  function setSession(authResponse) {
    localStorage.setItem(TOKEN_KEY, authResponse.token);
    localStorage.setItem(USER_KEY, JSON.stringify({
      userId: authResponse.userId,
      username: authResponse.username,
      avatarUrl: authResponse.avatarUrl,
      email: authResponse.email
    }));
  }

  function clearSession() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }

  function getUser() {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }

  async function request(path, options = {}) {
    const headers = {
      Accept: 'application/json',
      ...(options.body ? { 'Content-Type': 'application/json' } : {}),
      ...(options.headers || {})
    };

    const token = getToken();
    if (token) {
      headers.Authorization = `Bearer ${token}`;
    }

    const response = await fetch(path, {
      ...options,
      headers
    });

    if (response.status === 401) {
      window.dispatchEvent(new CustomEvent('streakhub:unauthorized'));
    }

    return response;
  }

  async function readJson(response) {
    if (response.status === 204) {
      return null;
    }

    const text = await response.text();
    if (!text) {
      return null;
    }

    return JSON.parse(text);
  }

  async function get(path) {
    const response = await request(path);
    const data = await readJson(response);
    if (!response.ok) {
      throw new Error(data?.title || data || `Request failed (${response.status})`);
    }
    return data;
  }

  async function send(path, method, body) {
    const response = await request(path, {
      method,
      body: body ? JSON.stringify(body) : undefined
    });
    const data = await readJson(response);
    if (!response.ok) {
      throw new Error(typeof data === 'string' ? data : data?.title || `Request failed (${response.status})`);
    }
    return data;
  }

  return {
    getToken,
    setSession,
    clearSession,
    getUser,
    get,
    post: (path, body) => send(path, 'POST', body),
    put: (path, body) => send(path, 'PUT', body),
    patch: (path, body) => send(path, 'PATCH', body),
    delete: (path) => send(path, 'DELETE')
  };
})();

window.StreakHubAuth = (() => {
  async function ensureAuthenticated() {
    let user = StreakHubApi.getUser();
    if (!user) {
      const modal = document.getElementById('login-modal');
      if (modal) {
        modal.classList.remove('hidden');
        modal.classList.add('flex');
        
        return new Promise((resolve) => {
          const form = document.getElementById('login-form');
          const cancel = document.getElementById('login-cancel');
          
          const handleAuth = (username) => {
            StreakHubApi.setSession({
              token: 'mock_token',
              userId: 1,
              username: username || 'demo',
              avatarUrl: 'https://avatars.githubusercontent.com/u/583231?v=4',
              email: 'demo@streakhub.com'
            });
            modal.classList.add('hidden');
            modal.classList.remove('flex');
            window.dispatchEvent(new CustomEvent('streakhub:authenticated'));
            resolve(true);
          };

          const onSubmit = (e) => {
            e.preventDefault();
            const username = document.getElementById('login-username')?.value.trim();
            form.removeEventListener('submit', onSubmit);
            cancel.removeEventListener('click', onCancel);
            handleAuth(username);
          };

          const onCancel = () => {
            form.removeEventListener('submit', onSubmit);
            cancel.removeEventListener('click', onCancel);
            modal.classList.add('hidden');
            modal.classList.remove('flex');
            resolve(false);
          };

          form.addEventListener('submit', onSubmit);
          cancel.addEventListener('click', onCancel);
        });
      } else {
        StreakHubApi.setSession({
          token: 'mock_token',
          userId: 1,
          username: 'demo',
          avatarUrl: 'https://avatars.githubusercontent.com/u/583231?v=4',
          email: 'demo@streakhub.com'
        });
        return true;
      }
    }
    return true;
  }

  return { ensureAuthenticated };
})();

window.StreakHubNotifications = (() => {
  let isPanelOpen = false;

  function getUserId() {
    return StreakHubApi.getUser()?.userId || 1;
  }

  function formatDisplayTime(dateString) {
    if (!dateString) return "";
    const d = new Date(dateString);
    return d.toLocaleTimeString("vi-VN", { hour: "2-digit", minute: "2-digit" }) + " " +
           d.toLocaleDateString("vi-VN", { day: "numeric", month: "numeric", year: "numeric" });
  }

  async function load() {
    const badge = document.getElementById('notifications-badge');
    const list = document.getElementById('notifications-list');
    const userId = getUserId();

    if (!badge || !list) return;

    try {
      const countRes = await StreakHubApi.get(`/api/Reminder/user/${userId}/unread-count`);
      const unreadCount = countRes?.count ?? 0;

      if (unreadCount > 0) {
        badge.textContent = unreadCount;
        badge.classList.remove('hidden');
      } else {
        badge.classList.add('hidden');
      }

      const reminders = await StreakHubApi.get(`/api/Reminder/user/${userId}?includeFuture=false`);
      
      list.innerHTML = '';

      if (!reminders || reminders.length === 0) {
        list.innerHTML = '<div class="p-4 text-center text-xs text-[#8b949e]">Không có thông báo mới</div>';
        return;
      }

      reminders.forEach((reminder) => {
        const item = document.createElement('div');
        item.className = `flex items-start gap-3 px-4 py-3 hover:bg-[#0d1117] border-b border-[#30363d]/40 transition-colors cursor-pointer ${reminder.isRead ? 'opacity-60' : ''}`;
        
        item.innerHTML = `
          <div class="mt-1 flex-shrink-0">
            <div style="width:8px;height:8px;border-radius:50%;background:${reminder.isRead ? 'transparent' : '#58a6ff'};border:${reminder.isRead ? '1px solid #30363d' : 'none'};"></div>
          </div>
          <div class="flex-1 min-w-0">
            <p class="text-sm text-[#c9d1d9] break-words">${reminder.title || 'Nhắc nhở công việc'}</p>
            <p class="text-xs text-[#8b949e] mt-0.5">${formatDisplayTime(reminder.notifyTime)}</p>
          </div>
          ${!reminder.isRead ? `
            <button class="mark-read-btn flex-shrink-0 text-xs text-[#58a6ff] hover:underline" data-id="${reminder.id}">
              Đọc
            </button>
          ` : ''}
        `;

        const btn = item.querySelector('.mark-read-btn');
        if (btn) {
          btn.addEventListener('click', async (e) => {
            e.stopPropagation();
            try {
              await StreakHubApi.post(`/api/Reminder/${reminder.id}/read`);
              await load();
            } catch (err) {
              console.error('Error marking reminder as read:', err);
            }
          });
        }

        list.appendChild(item);
      });
    } catch (err) {
      console.error('Error loading reminders:', err);
    }
  }

  function bindUi() {
    const toggle = document.getElementById('notifications-toggle');
    const panel = document.getElementById('notifications-panel');

    if (!toggle || !panel) return;

    toggle.addEventListener('click', (e) => {
      e.stopPropagation();
      isPanelOpen = !isPanelOpen;
      panel.classList.toggle('hidden', !isPanelOpen);
      if (isPanelOpen) {
        load();
      }
    });

    document.addEventListener('click', (e) => {
      if (panel && !panel.contains(e.target) && !toggle.contains(e.target)) {
        isPanelOpen = false;
        panel.classList.add('hidden');
      }
    });
  }

  function connectSignalR() {
    setInterval(load, 15000);
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', bindUi);
  } else {
    bindUi();
  }

  return {
    load,
    connectSignalR
  };
})();

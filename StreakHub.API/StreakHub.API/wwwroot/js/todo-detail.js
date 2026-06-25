window.StreakHubTodoDetail = (() => {
  let currentStatus = 'all';
  let currentSearch = '';
  let activeTodoId = null;
  let activeReminderId = null;
  let remindersCache = [];

  const taskDate = document.body.dataset.taskDate;

  function escapeHtml(value) {
    return String(value)
      .replaceAll('&', '&amp;')
      .replaceAll('<', '&lt;')
      .replaceAll('>', '&gt;')
      .replaceAll('"', '&quot;');
  }

  function formatRelative(dateValue) {
    const date = new Date(dateValue);
    const diffMs = Date.now() - date.getTime();
    const diffHours = Math.floor(diffMs / (1000 * 60 * 60));

    if (diffHours < 1) {
      return 'vừa xong';
    }
    if (diffHours < 24) {
      return `${diffHours} hours ago`;
    }

    return date.toLocaleDateString('vi-VN');
  }

  function formatDateTimeLocal(value) {
    const date = new Date(value);
    const offset = date.getTimezoneOffset();
    const local = new Date(date.getTime() - offset * 60 * 1000);
    return local.toISOString().slice(0, 16);
  }

  function tagClass(tag) {
    if (tag === 'coding') {
      return 'tag-coding';
    }
    if (tag === 'health') {
      return 'tag-health';
    }
    return '';
  }

  function renderReminderBadges(reminders) {
    if (!reminders?.length) {
      return '';
    }

    return reminders.map((reminder) => `
      <span class="inline-flex items-center gap-1 text-xs ${reminder.isSent ? 'reminder-badge-sent' : 'reminder-badge-pending'}">
        <svg height="12" viewBox="0 0 16 16" width="12" class="fill-current" aria-hidden="true"><path d="M8 16a2 2 0 0 0 1.985-1.75c.017-.137-.097-.25-.235-.25h-3.5c-.138 0-.252.113-.235.25A2 2 0 0 0 8 16ZM3 5a5 5 0 0 1 10 0v2.947c0 .05.015.098.042.139l1.703 2.555A1.519 1.519 0 0 1 13.482 13H2.518a1.516 1.516 0 0 1-1.263-2.359l1.703-2.555A.255.255 0 0 0 3 7.947Z"></path></svg>
        ${new Date(reminder.notifyTime).toLocaleString('vi-VN')} ${reminder.isSent ? '(sent)' : '(scheduled)'}
      </span>
    `).join('');
  }

  function renderTaskRow(todo, username) {
    const primaryTag = todo.tags[0] || 'general';
    const tagClasses = tagClass(primaryTag);

    return `
      <div class="flex items-start gap-3 p-3 border-b border-[#30363d] hover:bg-[#161b22] group transition-colors ${todo.isCompleted ? 'opacity-60' : ''}" data-todo-id="${todo.id}">
        <input type="checkbox" data-toggle-todo="${todo.id}" ${todo.isCompleted ? 'checked' : ''} class="mt-1 w-4 h-4 rounded border-[#30363d] bg-[#0d1117] checked:bg-[#238636] focus:ring-0 cursor-pointer" />
        <div class="flex-1 min-w-0">
          <button type="button" data-edit-todo="${todo.id}" class="text-base font-semibold ${todo.isCompleted ? 'text-[#8b949e] line-through' : 'text-[#c9d1d9] hover:text-[#58a6ff]'} block truncate text-left">
            ${escapeHtml(todo.title)}
          </button>
          <div class="mt-1 flex flex-wrap items-center gap-2 text-xs text-[#8b949e]">
            <span>#${todo.id} ${todo.isCompleted ? 'closed' : 'opened'} ${formatRelative(todo.createdAt)} by @${escapeHtml(username)}</span>
            ${renderReminderBadges(todo.reminders)}
          </div>
        </div>
        <div class="flex items-center gap-2">
          <span class="inline-flex items-center rounded-full border border-[#30363d] px-2 py-0.5 text-xs font-medium text-[#8b949e] ${tagClasses}">
            ${escapeHtml(primaryTag)}
          </span>
          <div class="relative">
            <button type="button" data-menu-toggle="${todo.id}" class="opacity-0 group-hover:opacity-100 p-1 rounded-md hover:bg-[#30363d] text-[#8b949e] hover:text-[#c9d1d9] transition-opacity">
              <svg height="16" viewBox="0 0 16 16" width="16" class="fill-current"><path d="M8 9a1.5 1.5 0 1 0 0-3 1.5 1.5 0 0 0 0 3ZM1.5 9a1.5 1.5 0 1 0 0-3 1.5 1.5 0 0 0 0 3Zm13 0a1.5 1.5 0 1 0 0-3 1.5 1.5 0 0 0 0 3Z"></path></svg>
            </button>
            <div id="task-menu-${todo.id}" class="hidden absolute right-0 mt-1 w-40 bg-[#161b22] border border-[#30363d] rounded-md shadow-lg z-10">
              <button type="button" data-edit-todo="${todo.id}" class="block w-full text-left px-3 py-2 text-sm hover:bg-[#30363d]">Edit</button>
              <button type="button" data-reminder-todo="${todo.id}" class="block w-full text-left px-3 py-2 text-sm hover:bg-[#30363d]">Reminders</button>
              <button type="button" data-delete-todo="${todo.id}" class="block w-full text-left px-3 py-2 text-sm text-[#f85149] hover:bg-[#30363d]">Delete</button>
            </div>
          </div>
        </div>
      </div>
    `;
  }

  async function loadTasks() {
    const params = new URLSearchParams({ date: taskDate });
    if (currentSearch) {
      params.set('search', currentSearch);
    }
    if (currentStatus !== 'all') {
      params.set('status', currentStatus);
    }

    const todos = await StreakHubApi.get(`/api/todos?${params.toString()}`);
    const user = StreakHubApi.getUser();
    const username = user?.username || 'user';

    const openCount = todos.filter((todo) => !todo.isCompleted).length;
    const closedCount = todos.filter((todo) => todo.isCompleted).length;

    document.getElementById('open-count').textContent = `${openCount} Open`;
    document.getElementById('closed-count').textContent = `${closedCount} Closed`;

    const list = document.getElementById('task-list');
    if (!list) {
      return;
    }

    if (!todos.length) {
      list.innerHTML = '<p class="p-4 text-sm text-[#8b949e]">Không có task cho ngày này.</p>';
      return;
    }

    list.innerHTML = todos.map((todo) => renderTaskRow(todo, username)).join('');
    bindTaskEvents();
  }

  function closeAllMenus() {
    document.querySelectorAll('[id^="task-menu-"]').forEach((menu) => menu.classList.add('hidden'));
  }

  function bindTaskEvents() {
    document.querySelectorAll('[data-toggle-todo]').forEach((checkbox) => {
      checkbox.addEventListener('change', async (event) => {
        const id = event.target.getAttribute('data-toggle-todo');
        await StreakHubApi.patch(`/api/todos/${id}/toggle`);
        await loadTasks();
      });
    });

    document.querySelectorAll('[data-delete-todo]').forEach((button) => {
      button.addEventListener('click', async () => {
        const id = button.getAttribute('data-delete-todo');
        if (!confirm('Delete this task?')) {
          return;
        }
        await StreakHubApi.delete(`/api/todos/${id}`);
        await loadTasks();
      });
    });

    document.querySelectorAll('[data-edit-todo]').forEach((button) => {
      button.addEventListener('click', async () => {
        const id = Number(button.getAttribute('data-edit-todo'));
        await openTaskModal(id);
        closeAllMenus();
      });
    });

    document.querySelectorAll('[data-reminder-todo]').forEach((button) => {
      button.addEventListener('click', async () => {
        const id = Number(button.getAttribute('data-reminder-todo'));
        await openReminderModal(id);
        closeAllMenus();
      });
    });

    document.querySelectorAll('[data-menu-toggle]').forEach((button) => {
      button.addEventListener('click', (event) => {
        event.stopPropagation();
        const id = button.getAttribute('data-menu-toggle');
        closeAllMenus();
        document.getElementById(`task-menu-${id}`)?.classList.remove('hidden');
      });
    });
  }

  function showModal(id) {
    document.getElementById(id)?.classList.remove('hidden');
  }

  function hideModal(id) {
    document.getElementById(id)?.classList.add('hidden');
  }

  async function openTaskModal(todoId = null) {
    activeTodoId = todoId;
    const titleInput = document.getElementById('task-title');
    const tagsInput = document.getElementById('task-tags');
    const completedInput = document.getElementById('task-completed');
    const modalTitle = document.getElementById('task-modal-title');

    if (todoId) {
      const todo = await StreakHubApi.get(`/api/todos/${todoId}`);
      titleInput.value = todo.title;
      tagsInput.value = todo.tags.join(', ');
      completedInput.checked = todo.isCompleted;
      modalTitle.textContent = 'Edit Task';
    } else {
      titleInput.value = '';
      tagsInput.value = '';
      completedInput.checked = false;
      modalTitle.textContent = 'New Task';
    }

    showModal('task-modal');
  }

  async function saveTask(event) {
    event.preventDefault();

    const title = document.getElementById('task-title').value.trim();
    const tags = document.getElementById('task-tags').value
      .split(',')
      .map((tag) => tag.trim())
      .filter(Boolean);
    const isCompleted = document.getElementById('task-completed').checked;

    if (activeTodoId) {
      await StreakHubApi.put(`/api/todos/${activeTodoId}`, {
        title,
        taskDate,
        isCompleted,
        tags
      });
    } else {
      await StreakHubApi.post('/api/todos', {
        title,
        taskDate,
        tags
      });
    }

    hideModal('task-modal');
    await loadTasks();
  }

  async function openReminderModal(todoId) {
    activeTodoId = todoId;
    activeReminderId = null;

    document.getElementById('reminder-notify-time').value = '';
    document.getElementById('reminder-form-title').textContent = 'Task Reminders';

    const userId = StreakHubApi.getUser()?.userId || 1;
    remindersCache = await StreakHubApi.get(`/api/Reminder/user/${userId}?includeFuture=true`);
    renderReminderList();
    showModal('reminder-modal');
  }

  function renderReminderList() {
    const list = document.getElementById('reminder-list');
    const taskReminders = remindersCache.filter((item) => item.taskId === activeTodoId);

    if (!taskReminders.length) {
      list.innerHTML = '<p class="text-sm text-[#8b949e]">Chưa có reminder cho task này.</p>';
      return;
    }

    list.innerHTML = taskReminders.map((reminder) => `
      <div class="flex items-center justify-between gap-3 border border-[#30363d] rounded-md p-3">
        <div>
          <p class="text-sm text-[#c9d1d9]">${new Date(reminder.notifyTime).toLocaleString('vi-VN')}</p>
          <p class="text-xs ${reminder.isSent ? 'reminder-badge-sent' : 'reminder-badge-pending'}">${reminder.isSent ? 'Sent' : 'Scheduled'}</p>
        </div>
        <div class="flex gap-2">
          <button type="button" data-edit-reminder="${reminder.id}" class="text-xs text-[#58a6ff] hover:underline">Edit</button>
          <button type="button" data-delete-reminder="${reminder.id}" class="text-xs text-[#f85149] hover:underline">Delete</button>
        </div>
      </div>
    `).join('');

    list.querySelectorAll('[data-edit-reminder]').forEach((button) => {
      button.addEventListener('click', () => {
        const reminder = remindersCache.find((item) => item.id === Number(button.getAttribute('data-edit-reminder')));
        if (!reminder) {
          return;
        }
        activeReminderId = reminder.id;
        document.getElementById('reminder-notify-time').value = formatDateTimeLocal(reminder.notifyTime);
      });
    });

    list.querySelectorAll('[data-delete-reminder]').forEach((button) => {
      button.addEventListener('click', async () => {
        const id = button.getAttribute('data-delete-reminder');
        await StreakHubApi.delete(`/api/Reminder/${id}`);
        const userId = StreakHubApi.getUser()?.userId || 1;
        remindersCache = await StreakHubApi.get(`/api/Reminder/user/${userId}?includeFuture=true`);
        renderReminderList();
        await loadTasks();
      });
    });
  }

  async function saveReminder(event) {
    event.preventDefault();
    const notifyTime = document.getElementById('reminder-notify-time').value;
    if (!notifyTime) {
      return;
    }

    if (activeReminderId) {
      await StreakHubApi.put(`/api/Reminder/${activeReminderId}`, {
        notifyTime: new Date(notifyTime).toISOString()
      });
    } else {
      await StreakHubApi.post('/api/Reminder', {
        taskId: activeTodoId,
        notifyTime: new Date(notifyTime).toISOString()
      });
    }

    activeReminderId = null;
    document.getElementById('reminder-notify-time').value = '';
    const userId = StreakHubApi.getUser()?.userId || 1;
    remindersCache = await StreakHubApi.get(`/api/Reminder/user/${userId}?includeFuture=true`);
    renderReminderList();
    await loadTasks();
  }

  function bindUi() {
    document.getElementById('new-task-btn')?.addEventListener('click', () => openTaskModal());
    document.getElementById('task-form')?.addEventListener('submit', saveTask);
    document.getElementById('task-modal-close')?.addEventListener('click', () => hideModal('task-modal'));
    document.getElementById('reminder-form')?.addEventListener('submit', saveReminder);
    document.getElementById('reminder-modal-close')?.addEventListener('click', () => hideModal('reminder-modal'));

    document.getElementById('task-search')?.addEventListener('input', async (event) => {
      currentSearch = event.target.value.trim();
      await loadTasks();
    });

    document.getElementById('filter-open')?.addEventListener('click', async (event) => {
      event.preventDefault();
      currentStatus = 'open';
      await loadTasks();
    });

    document.getElementById('filter-closed')?.addEventListener('click', async (event) => {
      event.preventDefault();
      currentStatus = 'closed';
      await loadTasks();
    });

    document.addEventListener('click', closeAllMenus);
  }

  async function init() {
    bindUi();

    const title = document.getElementById('page-title');
    if (title) {
      const date = new Date(`${taskDate}T00:00:00`);
      title.textContent = `Tasks for ${date.toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' })}`;
    }

    const authenticated = await StreakHubAuth.ensureAuthenticated();
    if (!authenticated) {
      window.addEventListener('streakhub:authenticated', init, { once: true });
      return;
    }

    await Promise.all([
      loadTasks(),
      StreakHubDnd.load(),
      StreakHubNotifications.load(),
      StreakHubNotifications.connectSignalR()
    ]);
  }

  document.addEventListener('DOMContentLoaded', init);

  return { init };
})();

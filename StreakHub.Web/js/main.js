// file: js/main.js

const BASE_URL = "http://localhost:5127/api";
const USER_ID = 1;

let activeDashboardReminders = [];
// DND settings initialized to null — state is loaded from the API, not preset
let dndSettings = null;

document.addEventListener("DOMContentLoaded", () => {
    loadUserProfile();
    loadTasks();
    setupDnd();
    setupReminders();
    setupReminderDashboard();
});

// ─────────────────────────────────────────────────────────────────────────────
// Helpers
// ─────────────────────────────────────────────────────────────────────────────

// Convert UTC date to local string formatted for datetime-local input
function formatDateTimeLocal(dateString) {
    if (!dateString) return "";
    const d = new Date(dateString);
    const tzoffset = d.getTimezoneOffset() * 60000;
    return new Date(d.getTime() - tzoffset).toISOString().slice(0, 16);
}

// Format date for display to user
function formatDisplayTime(dateString) {
    const d = new Date(dateString);
    return d.toLocaleTimeString("vi-VN", { hour: "2-digit", minute: "2-digit" }) + " " +
           d.toLocaleDateString("vi-VN", { day: "numeric", month: "numeric", year: "numeric" });
}

// ─────────────────────────────────────────────────────────────────────────────
// User Profile — loaded dynamically from API
// ─────────────────────────────────────────────────────────────────────────────

async function loadUserProfile() {
    const card = document.getElementById("user-profile-card");
    if (!card) return;

    try {
        const res = await fetch(`${BASE_URL}/user/${USER_ID}`);
        if (res.ok) {
            const user = await res.json();
            card.innerHTML = `
                <img
                    src="${user.avatarUrl || "https://avatars.githubusercontent.com/u/583231?v=4"}"
                    alt="Ảnh đại diện người dùng"
                    class="w-24 h-24 rounded-full border border-[#30363d] mb-3"
                    onerror="this.src='https://avatars.githubusercontent.com/u/583231?v=4'"
                />
                <h1 class="text-lg font-semibold text-[#c9d1d9]">${user.username || "User"}</h1>
                <p class="text-sm text-[#6e7681]">@${user.username || "unknown"}</p>
            `;
        } else {
            // API responded but user not found — show anonymous placeholder
            renderAnonymousProfile(card);
        }
    } catch {
        // API unavailable — show anonymous placeholder
        renderAnonymousProfile(card);
    }
}

function renderAnonymousProfile(card) {
    card.innerHTML = `
        <div class="w-24 h-24 rounded-full bg-[#30363d] flex items-center justify-center mb-3">
            <svg height="40" viewBox="0 0 16 16" width="40" class="fill-[#6e7681]" aria-hidden="true">
                <path d="M10.561 8.073a6.005 6.005 0 0 1 3.432 5.142.75.75 0 1 1-1.498.07 4.5 4.5 0 0 0-8.99 0 .75.75 0 0 1-1.498-.07 6.004 6.004 0 0 1 3.432-5.142 3.999 3.999 0 1 1 5.122 0ZM10.5 5a2.5 2.5 0 1 0-5 0 2.5 2.5 0 0 0 5 0Z"/>
            </svg>
        </div>
        <p class="text-sm text-[#6e7681]">Chưa đăng nhập</p>
    `;
}

// ─────────────────────────────────────────────────────────────────────────────
// Task Dropdown — loaded dynamically from API
// ─────────────────────────────────────────────────────────────────────────────

async function loadTasks() {
    const select = document.getElementById("schedule-task");
    if (!select) return;

    try {
        const res = await fetch(`${BASE_URL}/todo/user/${USER_ID}`);
        if (res.ok) {
            const tasks = await res.json();
            select.innerHTML = "";

            if (tasks.length === 0) {
                select.innerHTML = `<option value="" disabled selected>Chưa có công việc nào</option>`;
                return;
            }

            tasks.forEach(task => {
                const option = document.createElement("option");
                option.value = task.id;
                option.textContent = task.title;
                select.appendChild(option);
            });
        } else {
            select.innerHTML = `<option value="" disabled selected>Không thể tải danh sách công việc</option>`;
        }
    } catch {
        select.innerHTML = `<option value="" disabled selected>Không thể tải danh sách công việc</option>`;
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// DND Status Indicator
// ─────────────────────────────────────────────────────────────────────────────

// ─────────────────────────────────────────────────────────────────────────────
// DND Settings & Helpers
// ─────────────────────────────────────────────────────────────────────────────

let dndOn = false;

function checkDndActive() {
    const startInput = document.getElementById("dnd-start");
    const endInput = document.getElementById("dnd-end");
    const indicator = document.getElementById("dnd-active-indicator");

    if (!startInput || !endInput) return;

    if (!dndOn) {
        if (indicator) indicator.classList.add("hidden");
        return;
    }

    const now = new Date();
    const nowMin = now.getHours() * 60 + now.getMinutes();

    const [sh, sm] = startInput.value.split(":").map(Number);
    const [eh, em] = endInput.value.split(":").map(Number);
    const startMin = sh * 60 + sm;
    const endMin = eh * 60 + em;

    let active;
    if (startMin <= endMin) {
        active = nowMin >= startMin && nowMin < endMin;
    } else {
        active = nowMin >= startMin || nowMin < endMin;
    }
    if (indicator) indicator.classList.toggle("hidden", !active);
}

function applyDnd() {
    const toggle = document.getElementById("dnd-toggle");
    const statusLabel = document.getElementById("dnd-status-label");
    const timeRow = document.getElementById("dnd-time-selectors") || document.getElementById("dnd-time-row");

    if (!toggle) return;

    toggle.classList.toggle("on", dndOn);
    toggle.setAttribute("aria-checked", dndOn);

    const circle = toggle.querySelector("span");
    if (dndOn) {
        toggle.classList.remove("bg-[#30363d]");
        toggle.classList.add("bg-[#238636]");
        if (circle) {
            circle.classList.remove("translate-x-1", "bg-[#8b949e]");
            circle.classList.add("translate-x-5", "bg-white");
        }
        if (timeRow) {
            timeRow.classList.remove("opacity-50", "pointer-events-none");
        }
    } else {
        toggle.classList.remove("bg-[#238636]");
        toggle.classList.add("bg-[#30363d]");
        if (circle) {
            circle.classList.remove("translate-x-5", "bg-white");
            circle.classList.add("translate-x-1", "bg-[#8b949e]");
        }
        if (timeRow) {
            timeRow.classList.add("opacity-50", "pointer-events-none");
        }
    }

    if (statusLabel) {
        statusLabel.textContent = dndOn ? "ON" : "OFF";
        statusLabel.style.color = dndOn ? "#3fb950" : "#8b949e";
    }
    if (timeRow) timeRow.classList.toggle("visible", dndOn);
    checkDndActive();
}

async function setupDnd() {
    const toggle = document.getElementById("dnd-toggle");
    const startInput = document.getElementById("dnd-start");
    const endInput = document.getElementById("dnd-end");

    if (!toggle || !startInput || !endInput) return;

    // Load DND settings from API
    try {
        const res = await fetch(`${BASE_URL}/dnd/${USER_ID}`);
        if (res.ok) {
            const data = await res.json();
            dndOn = data.enabled;
            startInput.value = data.start;
            endInput.value = data.end;
            applyDnd();
        }
    } catch (err) {
        console.error("Lỗi khi tải cài đặt DND:", err);
    }

    const saveDnd = async () => {
        try {
            const res = await fetch(`${BASE_URL}/dnd/${USER_ID}`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    enabled: dndOn,
                    start: startInput.value,
                    end: endInput.value
                })
            });
            if (res.ok) {
                applyDnd();
                // Refresh reminder notifications to reflect new DND state
                updateRemindersUI();
                updateDashboardReminders();
            }
        } catch (err) {
            console.error("Lỗi khi cập nhật DND:", err);
        }
    };

    toggle.addEventListener("click", async () => {
        dndOn = !dndOn;
        await saveDnd();
    });

    toggle.addEventListener("keydown", async (e) => {
        if (e.key === "Enter" || e.key === " ") {
            e.preventDefault();
            dndOn = !dndOn;
            await saveDnd();
        }
    });

    startInput.addEventListener("change", saveDnd);
    endInput.addEventListener("change", saveDnd);
}

// ─────────────────────────────────────────────────────────────────────────────
// Notification Dropdown (Reminder Bell)
// ─────────────────────────────────────────────────────────────────────────────

async function setupReminders() {
    const bellBtn = document.getElementById("bell-btn");
    const reminderPanel = document.getElementById("reminder-panel");
    const markAllReadBtn = document.getElementById("mark-all-read-btn");

    if (!bellBtn || !reminderPanel || !markAllReadBtn) return;

    bellBtn.addEventListener("click", (e) => {
        e.stopPropagation();
        const isOpen = reminderPanel.classList.contains("open");
        reminderPanel.classList.toggle("open", !isOpen);
        bellBtn.setAttribute("aria-expanded", !isOpen);
    });

    document.addEventListener("click", (e) => {
        const container = document.getElementById("reminder-container");
        if (container && !container.contains(e.target)) {
            reminderPanel.classList.remove("open");
            bellBtn.setAttribute("aria-expanded", "false");
        }
    });

    markAllReadBtn.addEventListener("click", async () => {
        try {
            const res = await fetch(`${BASE_URL}/reminder/user/${USER_ID}/read-all`, { method: "POST" });
            if (res.ok) {
                await refreshAllReminders();
            }
        } catch (err) {
            console.error(err);
        }
    });

    updateRemindersUI();
    setInterval(() => {
        updateRemindersUI();
        checkDndActive();
    }, 15000);
}

async function updateRemindersUI() {
    const badge = document.getElementById("bell-badge");
    const list = document.getElementById("reminder-list");
    const unreadLabel = document.getElementById("unread-label");

    if (!badge || !list) return;

    try {
        // Fetch active reminder list for the dropdown
        const listRes = await fetch(`${BASE_URL}/reminder/user/${USER_ID}`);
        if (listRes.ok) {
            const activeReminders = await listRes.json();
            
            // Count unread
            const unreadCount = activeReminders.filter(r => !r.isRead).length;
            badge.textContent = unreadCount;
            badge.setAttribute("data-count", unreadCount);
            if (unreadLabel) {
                unreadLabel.textContent = unreadCount > 0 ? `${unreadCount} chưa đọc` : "Tất cả đã đọc";
            }

            list.innerHTML = "";

            if (activeReminders.length === 0) {
                list.innerHTML = `<li class="p-4 text-center text-xs text-[#8b949e]">Không có thông báo mới</li>`;
                return;
            }

            activeReminders.forEach(reminder => {
                const li = document.createElement("li");
                li.className = `flex items-start gap-3 px-4 py-3 hover:bg-[#0d1117] transition-colors cursor-pointer ${reminder.isRead ? "opacity-60" : ""}`;

                li.innerHTML = `
                    <div class="mt-0.5 flex-shrink-0">
                        <div style="width:8px;height:8px;border-radius:50%;background:${reminder.isRead ? 'transparent' : '#58a6ff'};border:${reminder.isRead ? '1px solid #30363d' : 'none'};margin-top:4px;"></div>
                    </div>
                    <div class="flex-1 min-w-0">
                        <p class="text-sm text-[#c9d1d9] truncate">${reminder.title}</p>
                        <p class="text-xs text-[#8b949e] mt-0.5">${formatDisplayTime(reminder.notifyTime)}</p>
                    </div>
                    ${!reminder.isRead ? `
                        <button class="mark-read-btn flex-shrink-0 text-xs text-[#58a6ff] hover:underline" onclick="markReminderAsRead(${reminder.id}, event)">
                            Đọc
                        </button>
                    ` : ""}
                `;
                list.appendChild(li);
            });
        }
    } catch (err) {
        console.error("Lỗi khi tải danh sách nhắc nhở:", err);
    }
}

// Global: mark a single reminder as read
window.markReminderAsRead = async function(reminderId, event) {
    if (event) event.stopPropagation();
    try {
        const res = await fetch(`${BASE_URL}/reminder/${reminderId}/read`, { method: "POST" });
        if (res.ok) {
            await refreshAllReminders();
        }
    } catch (err) {
        console.error(err);
    }
};

// ─────────────────────────────────────────────────────────────────────────────
// Reminder Dashboard (Create + List)
// ─────────────────────────────────────────────────────────────────────────────

async function setupReminderDashboard() {
    const timeInput = document.getElementById("schedule-time");
    const addBtn = document.getElementById("add-reminder-btn");

    if (!timeInput || !addBtn) return;

    // Set default reminder time to now + 5 minutes
    const defaultTime = new Date(Date.now() + 5 * 60 * 1000);
    timeInput.value = formatDateTimeLocal(defaultTime);

    // Create reminder on button click
    addBtn.addEventListener("click", async () => {
        const taskSelect = document.getElementById("schedule-task");

        // Guard: no task selected or no tasks loaded yet
        if (!taskSelect || !taskSelect.value || !timeInput.value) {
            alert("Vui lòng chọn công việc và thời gian nhắc nhở.");
            return;
        }

        const taskId = parseInt(taskSelect.value);
        if (isNaN(taskId)) {
            alert("Vui lòng chọn một công việc hợp lệ.");
            return;
        }

        const notifyTime = new Date(timeInput.value).toISOString();

        try {
            const res = await fetch(`${BASE_URL}/reminder`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ taskId, notifyTime })
            });

            if (res.ok) {
                // Reset time input to now + 5 minutes after successful creation
                const resetTime = new Date(Date.now() + 5 * 60 * 1000);
                timeInput.value = formatDateTimeLocal(resetTime);

                await refreshAllReminders();
            } else {
                alert("Không thể đặt nhắc nhở. Vui lòng kiểm tra lại.");
            }
        } catch (err) {
            console.error("Lỗi khi đặt nhắc nhở:", err);
        }
    });

    // Load existing reminders on page load
    updateDashboardReminders();
}

async function updateDashboardReminders() {
    const tableBody = document.getElementById("reminders-table-body");
    const countBadge = document.getElementById("reminders-count");

    if (!tableBody || !countBadge) return;

    try {
        const res = await fetch(`${BASE_URL}/reminder/user/${USER_ID}?includeFuture=true`);
        if (res.ok) {
            activeDashboardReminders = await res.json();
            countBadge.textContent = `${activeDashboardReminders.length} nhắc nhở`;
            tableBody.innerHTML = "";

            if (activeDashboardReminders.length === 0) {
                tableBody.innerHTML = `
                    <tr>
                        <td colspan="4" class="py-8 text-center text-xs text-[#8b949e]">
                            Chưa có lịch nhắc nhở nào được thiết lập. Hãy tạo một cái ở trên!
                        </td>
                    </tr>
                `;
                return;
            }

            const now = new Date();

            activeDashboardReminders.forEach(reminder => {
                const tr = document.createElement("tr");
                tr.className = "border-b border-[#30363d]/40 hover:bg-[#161b22]/40 transition-colors";

                const isFuture = new Date(reminder.notifyTime) > now;
                const statusBadge = isFuture
                    ? `<span class="px-2 py-0.5 rounded-full text-xs font-medium bg-[#388bfd]/10 text-[#58a6ff] border border-[#388bfd]/20">Đang lên lịch</span>`
                    : `<span class="px-2 py-0.5 rounded-full text-xs font-medium bg-[#30363d] text-[#8b949e]">Đã gửi</span>`;

                tr.innerHTML = `
                    <td class="py-3 px-4 text-sm font-medium text-[#c9d1d9] max-w-[200px] truncate">${reminder.title}</td>
                    <td class="py-3 px-4">
                        <div id="time-display-${reminder.id}">
                            <span class="text-[#8b949e] text-xs">${formatDisplayTime(reminder.notifyTime)}</span>
                        </div>
                        <div id="time-edit-${reminder.id}" class="hidden flex items-center gap-1.5 mt-1">
                            <input type="datetime-local" id="input-edit-${reminder.id}" class="bg-[#0d1117] border border-[#30363d] rounded px-1.5 py-0.5 text-xs text-[#c9d1d9] focus:outline-none focus:border-[#58a6ff]">
                            <button class="px-2 py-0.5 bg-[#238636] hover:bg-[#2ea043] text-white rounded text-[10px] font-semibold" onclick="saveReschedule(${reminder.id})">Lưu</button>
                            <button class="px-2 py-0.5 bg-[#21262d] hover:bg-[#30363d] border border-[#30363d] text-[#c9d1d9] rounded text-[10px]" onclick="toggleEditTime(${reminder.id}, false)">Hủy</button>
                        </div>
                    </td>
                    <td class="py-3 px-4">${statusBadge}</td>
                    <td class="py-3 px-4 text-right">
                        <div class="flex items-center justify-end gap-3">
                            ${isFuture ? `
                                <button class="text-xs text-[#58a6ff] hover:underline" onclick="toggleEditTime(${reminder.id}, true)">Đổi lịch</button>
                            ` : ""}
                            <button class="text-xs text-[#f78166] hover:underline" onclick="deleteDashboardReminder(${reminder.id})">Xóa</button>
                        </div>
                    </td>
                `;
                tableBody.appendChild(tr);
            });
        }
    } catch (err) {
        console.error("Lỗi khi tải danh sách nhắc nhở trong Dashboard:", err);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Inline table actions (called from dynamically rendered HTML)
// ─────────────────────────────────────────────────────────────────────────────

window.toggleEditTime = function(reminderId, show) {
    const display = document.getElementById(`time-display-${reminderId}`);
    const edit = document.getElementById(`time-edit-${reminderId}`);
    const input = document.getElementById(`input-edit-${reminderId}`);
    if (!display || !edit) return;

    if (show) {
        display.classList.add("hidden");
        edit.classList.remove("hidden");
        // Pre-fill with existing reminder time
        const r = activeDashboardReminders.find(rem => rem.id === reminderId);
        if (r && input) {
            input.value = formatDateTimeLocal(r.notifyTime);
        }
    } else {
        display.classList.remove("hidden");
        edit.classList.add("hidden");
    }
};

window.saveReschedule = async function(reminderId) {
    const input = document.getElementById(`input-edit-${reminderId}`);
    if (!input || !input.value) return;

    const utcTime = new Date(input.value).toISOString();

    try {
        const res = await fetch(`${BASE_URL}/reminder/${reminderId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ notifyTime: utcTime })
        });
        if (res.ok) {
            await refreshAllReminders();
        } else {
            alert("Không thể đổi lịch nhắc nhở. Vui lòng kiểm tra lại.");
        }
    } catch (err) {
        console.error("Lỗi khi đổi lịch nhắc nhở:", err);
    }
};

window.deleteDashboardReminder = async function(reminderId) {
    if (!confirm("Bạn có chắc chắn muốn xóa nhắc nhở này?")) return;
    try {
        const res = await fetch(`${BASE_URL}/reminder/${reminderId}`, {
            method: "DELETE"
        });
        if (res.ok) {
            await refreshAllReminders();
        }
    } catch (err) {
        console.error("Lỗi khi xóa nhắc nhở:", err);
    }
};

// ─────────────────────────────────────────────────────────────────────────────
// Shared refresh helper
// ─────────────────────────────────────────────────────────────────────────────

async function refreshAllReminders() {
    await updateRemindersUI();
    await updateDashboardReminders();
}
// explore.js — initialises DND and Notifications on the Explore page

document.addEventListener('DOMContentLoaded', async () => {
  await StreakHubAuth.ensureAuthenticated();
  await Promise.all([
    StreakHubDnd.load(),
    StreakHubNotifications.load(),
    StreakHubNotifications.connectSignalR()
  ]);
});

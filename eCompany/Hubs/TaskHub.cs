using Microsoft.AspNetCore.SignalR;

namespace eCompany.Hubs
{
    public class TaskHub : Hub
    {

        public async Task UpdateTaskTable(string employeeId)
        {

            await Clients.All.SendAsync("ReceiveId", employeeId);
        }

        public async Task DeleteTask(string id)
        {
            await Clients.All.SendAsync("ReceiveDelete", id);
        }
    }
}

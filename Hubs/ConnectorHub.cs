using Microsoft.AspNetCore.SignalR;

namespace TheSocialCebu_Capstone.Hubs
{
    public class ConnectorHub: Hub
    {
        // Group By Table
        public Task JoinBranch(string table)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, table);
        }

        // Leave a specific branch group
        public Task LeaveBranch(string table)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, table);
        }
    }
}

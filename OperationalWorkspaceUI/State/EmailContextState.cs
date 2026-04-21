using OperationalWorkspaceApplication.DTOs;

namespace OperationalWorkspaceUI.State
{
   
    public class EmailContextState
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; NotifyStateChanged(); }
        }


        private EmailInsightDto? _currentEmail;
        private ClientDto? _matchedClient;
        private List<OrderDto> _linkedOrders = new();
        private List<TaskDto> _linkedTasks = new();

        // Event for components to subscribe to when state changes
        public event Action? OnChange;

        public EmailInsightDto? CurrentEmail
        {
            get => _currentEmail;
            set
            {
                _currentEmail = value;
                NotifyStateChanged();
            }
        }

        public ClientDto? MatchedClient
        {
            get => _matchedClient;
            set
            {
                _matchedClient = value;
                NotifyStateChanged();
            }
        }

        public List<OrderDto> LinkedOrders
        {
            get => _linkedOrders;
            set
            {
                _linkedOrders = value ?? new List<OrderDto>();
                NotifyStateChanged();
            }
        }

        public List<TaskDto> LinkedTasks
        {
            get => _linkedTasks;
            set
            {
                _linkedTasks = value ?? new List<TaskDto>();
                NotifyStateChanged();
            }
        }

        // Optional: the raw initial mailbox item loaded from Office.js
        public string InitialSubject { get; set; } = string.Empty;
        public string InitialBody { get; set; } = string.Empty;
        public string InitialFrom { get; set; } = string.Empty;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}

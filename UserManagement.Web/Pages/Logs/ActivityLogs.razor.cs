using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Web.Pages.Logs
{
    public partial class ActivityLogs : ComponentBase
    {
        [Inject] private IActivityLogService ActivityLogService { get; set; } = null!;

        private List<ActivityLog> allLogs = new();
        private List<ActivityLog> filteredLogs = new();

        private string? searchTerm;
        private DateTime? startDate;
        private DateTime? endDate;
        private string userIdFilterText = string.Empty;
        private string actionFilter = "All";

        private int totalLogs;
        private int filteredCount;
        private int totalPages;
        private int currentPage = 1;
        private const int pageSize = 10;

        private bool showDetails;
        private ActivityLog? selectedLog;

        private IEnumerable<ActivityLog> currentPageItems =>
            filteredLogs
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize);

        protected override async Task OnInitializedAsync()
        {
            allLogs = await ActivityLogService.GetAllAsync();
            totalLogs = allLogs.Count;
            ApplyFilters();
        }

        private void OnFiltersChanged() => ApplyFilters();

        private void ApplyFilters()
        {
            IEnumerable<ActivityLog> query = allLogs;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLowerInvariant();
                query = query.Where(l =>
                    !string.IsNullOrEmpty(l.UserName) &&
                    l.UserName.ToLower().Contains(term));
            }

            if (startDate.HasValue)
                query = query.Where(l => l.Timestamp >= startDate.Value.Date);

            if (endDate.HasValue)
            {
                var inclusiveEnd = endDate.Value.Date.AddDays(1);
                query = query.Where(l => l.Timestamp < inclusiveEnd);
            }

            if (actionFilter != "All")
                query = query.Where(l => l.Action == actionFilter);

            filteredLogs = query
                .OrderByDescending(l => l.Timestamp)
                .ToList();

            filteredCount = filteredLogs.Count;
            totalPages = filteredCount == 0
                ? 0
                : (int)Math.Ceiling(filteredCount / (double)pageSize);

            currentPage = totalPages == 0 ? 0 : 1;
        }

        private void ChangePage(int page)
        {
            if (page < 1 || page > totalPages)
                return;

            currentPage = page;
        }

        private void OpenDetails(ActivityLog log)
        {
            selectedLog = log;
            showDetails = true;
        }

        private void CloseDetails()
        {
            showDetails = false;
            selectedLog = null;
        }
    }
}

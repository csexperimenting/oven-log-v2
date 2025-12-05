using Microsoft.EntityFrameworkCore;
using OvenLog.Domain.Entities;
using OvenLog.Infrastructure.Data;

namespace OvenLog.Infrastructure.Services;

public class OvenLogService
{
    private readonly OvenLogDbContext _context;

    public OvenLogService(OvenLogDbContext context)
    {
        _context = context;
    }

    // TRAK Operations
    public async Task<Trak?> GetTrakByIdAsync(string trakId)
    {
        return await _context.Traks
            .Include(t => t.Part)
            .Include(t => t.Events.Where(e => e.DateOut == null))
            .FirstOrDefaultAsync(t => t.TrakId == trakId);
    }

    public async Task<List<Trak>> GetAvailableTraksAsync()
    {
        var traksInOvens = await _context.OvenEvents
            .Where(e => e.DateOut == null)
            .Select(e => e.TrakId)
            .ToListAsync();

        return await _context.Traks
            .Include(t => t.Part)
            .Where(t => !traksInOvens.Contains(t.Id))
            .ToListAsync();
    }

    public async Task<bool> IsTrakInOvenAsync(int trakId)
    {
        return await _context.OvenEvents
            .AnyAsync(e => e.TrakId == trakId && e.DateOut == null);
    }

    public async Task<Trak> CreateOrGetTrakAsync(string trakId, string serialNumber = "", string workOrder = "", int? partId = null)
    {
        var existing = await _context.Traks.FirstOrDefaultAsync(t => t.TrakId == trakId);
        if (existing != null)
            return existing;

        var trak = new Trak
        {
            TrakId = trakId,
            SerialNumber = serialNumber,
            WorkOrder = workOrder,
            PartId = partId
        };
        _context.Traks.Add(trak);
        await _context.SaveChangesAsync();
        return trak;
    }

    // Box/Oven Operations
    public async Task<List<Box>> GetAllBoxesAsync()
    {
        return await _context.Boxes
            .Include(b => b.Location)
            .Include(b => b.Model)
                .ThenInclude(m => m.Manufacturer)
            .Include(b => b.BoxType)
            .OrderBy(b => b.BoxType.Name)
            .ThenBy(b => b.ToolId)
            .ToListAsync();
    }

    public async Task<List<Box>> GetUserSelectedBoxesAsync(int userId)
    {
        var selectedBoxIds = await _context.UserOvenSelections
            .Where(s => s.UserId == userId)
            .Select(s => s.BoxId)
            .ToListAsync();

        if (!selectedBoxIds.Any())
        {
            return await GetAllBoxesAsync();
        }

        return await _context.Boxes
            .Include(b => b.Location)
            .Include(b => b.Model)
                .ThenInclude(m => m.Manufacturer)
            .Include(b => b.BoxType)
            .Where(b => selectedBoxIds.Contains(b.Id))
            .OrderBy(b => b.BoxType.Name)
            .ThenBy(b => b.ToolId)
            .ToListAsync();
    }

    public async Task<Box?> GetBoxByToolIdAsync(string toolId)
    {
        return await _context.Boxes
            .Include(b => b.Location)
            .Include(b => b.Model)
            .Include(b => b.BoxType)
            .FirstOrDefaultAsync(b => b.ToolId == toolId);
    }

    public async Task<bool> IsBoxWarmingUpAsync(int boxId)
    {
        var box = await _context.Boxes.FindAsync(boxId);
        if (box == null || box.HasDigitalDisplay || !box.WarmUpTime.HasValue)
            return false;

        var lastOnEvent = await _context.OnEvents
            .Where(e => e.BoxId == boxId)
            .OrderByDescending(e => e.ActualRecordedTime)
            .FirstOrDefaultAsync();

        if (lastOnEvent == null)
            return true; // No on event recorded, can't add TRAK

        return lastOnEvent.IsWarmingUp(box.WarmUpTime.Value);
    }

    // Oven Event Operations
    public async Task<List<OvenEvent>> GetTraksInOvensAsync()
    {
        return await _context.OvenEvents
            .Include(e => e.Box)
                .ThenInclude(b => b.Location)
            .Include(e => e.Trak)
                .ThenInclude(t => t.Part)
            .Include(e => e.UserIn)
            .Include(e => e.Application)
            .Where(e => e.DateOut == null)
            .OrderBy(e => e.DateIn)
            .ToListAsync();
    }

    public async Task<OvenEvent?> GetEventByTrakInOvenAsync(int trakId)
    {
        return await _context.OvenEvents
            .Include(e => e.Box)
            .Include(e => e.Trak)
            .Include(e => e.UserIn)
            .Include(e => e.Application)
            .FirstOrDefaultAsync(e => e.TrakId == trakId && e.DateOut == null);
    }

    public async Task<OvenEvent> AddTrakToOvenAsync(
        int trakId,
        int boxId,
        int userId,
        double temperature,
        double bakeTimeHours,
        int quantity,
        DateTime startTime,
        int? applicationId = null,
        string? note = null)
    {
        var ovenEvent = new OvenEvent
        {
            TrakId = trakId,
            BoxId = boxId,
            UserInId = userId,
            DateIn = startTime,
            Temperature = temperature,
            BakeTimeHours = bakeTimeHours,
            Quantity = quantity,
            ApplicationId = applicationId,
            Note = note
        };

        _context.OvenEvents.Add(ovenEvent);
        await _context.SaveChangesAsync();

        return await _context.OvenEvents
            .Include(e => e.Box)
                .ThenInclude(b => b.Location)
            .Include(e => e.Trak)
                .ThenInclude(t => t.Part)
            .Include(e => e.UserIn)
            .Include(e => e.Application)
            .FirstAsync(e => e.Id == ovenEvent.Id);
    }

    public async Task<bool> RemoveTrakFromOvenAsync(int eventId, int userId)
    {
        var ovenEvent = await _context.OvenEvents.FindAsync(eventId);
        if (ovenEvent == null || ovenEvent.DateOut.HasValue)
            return false;

        ovenEvent.DateOut = DateTime.UtcNow;
        ovenEvent.UserOutId = userId;
        await _context.SaveChangesAsync();
        return true;
    }

    // History Operations
    public async Task<List<OvenEvent>> GetTrakHistoryAsync(int trakId)
    {
        return await _context.OvenEvents
            .Include(e => e.Box)
                .ThenInclude(b => b.Location)
            .Include(e => e.Trak)
            .Include(e => e.UserIn)
            .Include(e => e.UserOut)
            .Include(e => e.Application)
            .Where(e => e.TrakId == trakId)
            .OrderByDescending(e => e.DateIn)
            .ToListAsync();
    }

    public async Task<List<OvenEvent>> GetRecentActivityAsync(int hours = 24)
    {
        var cutoff = DateTime.UtcNow.AddHours(-hours);
        return await _context.OvenEvents
            .Include(e => e.Box)
                .ThenInclude(b => b.Location)
            .Include(e => e.Trak)
                .ThenInclude(t => t.Part)
            .Include(e => e.UserIn)
            .Include(e => e.UserOut)
            .Include(e => e.Application)
            .Where(e => e.DateIn >= cutoff || (e.DateOut.HasValue && e.DateOut >= cutoff))
            .OrderByDescending(e => e.DateIn)
            .ToListAsync();
    }

    public async Task<List<OvenEvent>> GetAllEventsAsync()
    {
        return await _context.OvenEvents
            .Include(e => e.Box)
                .ThenInclude(b => b.Location)
            .Include(e => e.Trak)
                .ThenInclude(t => t.Part)
            .Include(e => e.UserIn)
            .Include(e => e.UserOut)
            .Include(e => e.Application)
            .OrderByDescending(e => e.DateIn)
            .ToListAsync();
    }

    // On Event Operations (for ovens without digital displays)
    public async Task RecordOvenOnAsync(int boxId, int userId, DateTime startTime)
    {
        var onEvent = new OnEvent
        {
            BoxId = boxId,
            UserId = userId,
            IntendedStartTime = startTime,
            ActualRecordedTime = DateTime.UtcNow
        };

        _context.OnEvents.Add(onEvent);
        await _context.SaveChangesAsync();
    }

    // Application Operations
    public async Task<List<Application>> GetAllApplicationsAsync()
    {
        return await _context.Applications
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<Application?> GetApplicationByBarcodeAsync(string barcode)
    {
        return await _context.Applications
            .FirstOrDefaultAsync(a => a.Barcode == barcode);
    }

    // Standard Time Operations
    public async Task<List<StandardTime>> GetAllStandardTimesAsync()
    {
        return await _context.StandardTimes
            .OrderBy(t => t.Hours)
            .ToListAsync();
    }

    public async Task<StandardTime?> GetStandardTimeByBarcodeAsync(string barcode)
    {
        return await _context.StandardTimes
            .FirstOrDefaultAsync(t => t.Barcode == barcode);
    }

    // User Operations
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Aliases)
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync();
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        var user = await _context.Users
            .Include(u => u.DedicatedBox)
            .FirstOrDefaultAsync(u => u.Login == login);

        if (user != null)
            return user;

        // Check aliases
        var alias = await _context.UserAliases
            .Include(a => a.User)
                .ThenInclude(u => u.DedicatedBox)
            .FirstOrDefaultAsync(a => a.UserName == login || a.Alias == login);

        return alias?.User;
    }

    public async Task<User?> GetUserByBadgeAsync(string badge)
    {
        return await _context.Users
            .Include(u => u.DedicatedBox)
            .FirstOrDefaultAsync(u => u.Badge == badge);
    }

    // User Oven Selection Operations
    public async Task SaveUserOvenSelectionsAsync(int userId, List<int> boxIds)
    {
        var existing = await _context.UserOvenSelections
            .Where(s => s.UserId == userId)
            .ToListAsync();

        _context.UserOvenSelections.RemoveRange(existing);

        var newSelections = boxIds.Select(boxId => new UserOvenSelection
        {
            UserId = userId,
            BoxId = boxId
        });

        await _context.UserOvenSelections.AddRangeAsync(newSelections);
        await _context.SaveChangesAsync();
    }

    public async Task<List<int>> GetUserOvenSelectionsAsync(int userId)
    {
        return await _context.UserOvenSelections
            .Where(s => s.UserId == userId)
            .Select(s => s.BoxId)
            .ToListAsync();
    }
}

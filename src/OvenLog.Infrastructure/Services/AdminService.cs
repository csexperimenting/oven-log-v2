using Microsoft.EntityFrameworkCore;
using OvenLog.Domain.Entities;
using OvenLog.Infrastructure.Data;

namespace OvenLog.Infrastructure.Services;

public class AdminService
{
    private readonly OvenLogDbContext _context;

    public AdminService(OvenLogDbContext context)
    {
        _context = context;
    }

    // Manufacturer Operations
    public async Task<List<Manufacturer>> GetAllManufacturersAsync()
    {
        return await _context.Manufacturers
            .Include(m => m.Models)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<Manufacturer> CreateManufacturerAsync(string name)
    {
        var manufacturer = new Manufacturer { Name = name };
        _context.Manufacturers.Add(manufacturer);
        await _context.SaveChangesAsync();
        return manufacturer;
    }

    public async Task<bool> UpdateManufacturerAsync(int id, string name)
    {
        var manufacturer = await _context.Manufacturers.FindAsync(id);
        if (manufacturer == null) return false;
        manufacturer.Name = name;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteManufacturerAsync(int id)
    {
        var manufacturer = await _context.Manufacturers.FindAsync(id);
        if (manufacturer == null) return false;
        _context.Manufacturers.Remove(manufacturer);
        await _context.SaveChangesAsync();
        return true;
    }

    // Model Operations
    public async Task<List<Model>> GetAllModelsAsync()
    {
        return await _context.Models
            .Include(m => m.Manufacturer)
            .OrderBy(m => m.Manufacturer.Name)
            .ThenBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<Model> CreateModelAsync(string name, int manufacturerId)
    {
        var model = new Model { Name = name, ManufacturerId = manufacturerId };
        _context.Models.Add(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<bool> UpdateModelAsync(int id, string name, int manufacturerId)
    {
        var model = await _context.Models.FindAsync(id);
        if (model == null) return false;
        model.Name = name;
        model.ManufacturerId = manufacturerId;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteModelAsync(int id)
    {
        var model = await _context.Models.FindAsync(id);
        if (model == null) return false;
        _context.Models.Remove(model);
        await _context.SaveChangesAsync();
        return true;
    }

    // BoxType Operations
    public async Task<List<BoxType>> GetAllBoxTypesAsync()
    {
        return await _context.BoxTypes
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<BoxType> CreateBoxTypeAsync(string name)
    {
        var boxType = new BoxType { Name = name };
        _context.BoxTypes.Add(boxType);
        await _context.SaveChangesAsync();
        return boxType;
    }

    public async Task<bool> UpdateBoxTypeAsync(int id, string name)
    {
        var boxType = await _context.BoxTypes.FindAsync(id);
        if (boxType == null) return false;
        boxType.Name = name;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBoxTypeAsync(int id)
    {
        var boxType = await _context.BoxTypes.FindAsync(id);
        if (boxType == null) return false;
        _context.BoxTypes.Remove(boxType);
        await _context.SaveChangesAsync();
        return true;
    }

    // Location Operations
    public async Task<List<Location>> GetAllLocationsAsync()
    {
        return await _context.Locations
            .OrderBy(l => l.Name)
            .ToListAsync();
    }

    public async Task<Location> CreateLocationAsync(string name)
    {
        var location = new Location { Name = name };
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
        return location;
    }

    public async Task<bool> UpdateLocationAsync(int id, string name)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return false;
        location.Name = name;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteLocationAsync(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return false;
        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();
        return true;
    }

    // Box Operations
    public async Task<Box> CreateBoxAsync(Box box)
    {
        _context.Boxes.Add(box);
        await _context.SaveChangesAsync();
        return box;
    }

    public async Task<bool> UpdateBoxAsync(Box box)
    {
        var existing = await _context.Boxes.FindAsync(box.Id);
        if (existing == null) return false;

        existing.ToolId = box.ToolId;
        existing.DefaultTemperature = box.DefaultTemperature;
        existing.TemperatureScale = box.TemperatureScale;
        existing.WarmUpTime = box.WarmUpTime;
        existing.HasDigitalDisplay = box.HasDigitalDisplay;
        existing.LocationId = box.LocationId;
        existing.ModelId = box.ModelId;
        existing.BoxTypeId = box.BoxTypeId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBoxAsync(int id)
    {
        var box = await _context.Boxes.FindAsync(id);
        if (box == null) return false;
        _context.Boxes.Remove(box);
        await _context.SaveChangesAsync();
        return true;
    }

    // User Operations
    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        var existing = await _context.Users.FindAsync(user.Id);
        if (existing == null) return false;

        existing.FirstName = user.FirstName;
        existing.MiddleName = user.MiddleName;
        existing.LastName = user.LastName;
        existing.Badge = user.Badge;
        existing.Login = user.Login;
        existing.IsActive = user.IsActive;
        existing.DedicatedBoxId = user.DedicatedBoxId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    // UserAlias Operations
    public async Task<List<UserAlias>> GetUserAliasesAsync(int userId)
    {
        return await _context.UserAliases
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserAlias> CreateUserAliasAsync(int userId, string alias, string userName)
    {
        var userAlias = new UserAlias
        {
            UserId = userId,
            Alias = alias,
            UserName = userName
        };
        _context.UserAliases.Add(userAlias);
        await _context.SaveChangesAsync();
        return userAlias;
    }

    public async Task<bool> DeleteUserAliasAsync(int id)
    {
        var alias = await _context.UserAliases.FindAsync(id);
        if (alias == null) return false;
        _context.UserAliases.Remove(alias);
        await _context.SaveChangesAsync();
        return true;
    }

    // Application Operations
    public async Task<Application> CreateApplicationAsync(Application application)
    {
        _context.Applications.Add(application);
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<bool> UpdateApplicationAsync(Application application)
    {
        var existing = await _context.Applications.FindAsync(application.Id);
        if (existing == null) return false;

        existing.Name = application.Name;
        existing.DefaultBakeTime = application.DefaultBakeTime;
        existing.DefaultTemperature = application.DefaultTemperature;
        existing.MinTemperature = application.MinTemperature;
        existing.MaxTemperature = application.MaxTemperature;
        existing.Barcode = application.Barcode;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteApplicationAsync(int id)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null) return false;
        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();
        return true;
    }

    // StandardTime Operations
    public async Task<StandardTime> CreateStandardTimeAsync(StandardTime standardTime)
    {
        _context.StandardTimes.Add(standardTime);
        await _context.SaveChangesAsync();
        return standardTime;
    }

    public async Task<bool> UpdateStandardTimeAsync(StandardTime standardTime)
    {
        var existing = await _context.StandardTimes.FindAsync(standardTime.Id);
        if (existing == null) return false;

        existing.Barcode = standardTime.Barcode;
        existing.Description = standardTime.Description;
        existing.Hours = standardTime.Hours;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteStandardTimeAsync(int id)
    {
        var standardTime = await _context.StandardTimes.FindAsync(id);
        if (standardTime == null) return false;
        _context.StandardTimes.Remove(standardTime);
        await _context.SaveChangesAsync();
        return true;
    }

    // Part Operations
    public async Task<List<Part>> GetAllPartsAsync()
    {
        return await _context.Parts
            .OrderBy(p => p.PartNumber)
            .ToListAsync();
    }

    public async Task<Part> CreatePartAsync(string partNumber, string description)
    {
        var part = new Part { PartNumber = partNumber, Description = description };
        _context.Parts.Add(part);
        await _context.SaveChangesAsync();
        return part;
    }

    // Relink User Operation (for handling duplicate users after login changes)
    public async Task<bool> RelinkUserAsync(int oldUserId, int newUserId)
    {
        var oldUser = await _context.Users.FindAsync(oldUserId);
        var newUser = await _context.Users.FindAsync(newUserId);
        
        if (oldUser == null || newUser == null)
            return false;

        // Update all events where old user was UserIn
        var eventsIn = await _context.OvenEvents
            .Where(e => e.UserInId == oldUserId)
            .ToListAsync();
        foreach (var evt in eventsIn)
            evt.UserInId = newUserId;

        // Update all events where old user was UserOut
        var eventsOut = await _context.OvenEvents
            .Where(e => e.UserOutId == oldUserId)
            .ToListAsync();
        foreach (var evt in eventsOut)
            evt.UserOutId = newUserId;

        // Update all OnEvents
        var onEvents = await _context.OnEvents
            .Where(e => e.UserId == oldUserId)
            .ToListAsync();
        foreach (var evt in onEvents)
            evt.UserId = newUserId;

        // Move aliases
        var aliases = await _context.UserAliases
            .Where(a => a.UserId == oldUserId)
            .ToListAsync();
        foreach (var alias in aliases)
            alias.UserId = newUserId;

        // Add old user's login as an alias for new user
        var newAlias = new UserAlias
        {
            UserId = newUserId,
            Alias = oldUser.Login,
            UserName = newUser.Login
        };
        _context.UserAliases.Add(newAlias);

        // Delete old user
        _context.Users.Remove(oldUser);

        await _context.SaveChangesAsync();
        return true;
    }
}

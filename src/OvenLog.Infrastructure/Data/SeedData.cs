using Microsoft.EntityFrameworkCore;
using OvenLog.Domain.Entities;

namespace OvenLog.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(OvenLogDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.BoxTypes.AnyAsync())
            return;

        // Box Types
        var boxTypes = new List<BoxType>
        {
            new() { Name = "Oven" },
            new() { Name = "Dry Box" },
            new() { Name = "Freezer" },
            new() { Name = "Refrigerator" },
            new() { Name = "Solder Pot" },
            new() { Name = "Cart" }
        };
        await context.BoxTypes.AddRangeAsync(boxTypes);
        await context.SaveChangesAsync();

        // Locations
        var locations = new List<Location>
        {
            new() { Name = "F2" },
            new() { Name = "F3" },
            new() { Name = "Lab 1" },
            new() { Name = "Lab 2" },
            new() { Name = "Production Floor" },
            new() { Name = "Shelf" }
        };
        await context.Locations.AddRangeAsync(locations);
        await context.SaveChangesAsync();

        // Manufacturers
        var manufacturers = new List<Manufacturer>
        {
            new() { Name = "Sheldon" },
            new() { Name = "Despatch" },
            new() { Name = "Blue M" },
            new() { Name = "Precision" },
            new() { Name = "Associated" },
            new() { Name = "McDry" },
            new() { Name = "Dr. Storage" },
            new() { Name = "Erecta" }
        };
        await context.Manufacturers.AddRangeAsync(manufacturers);
        await context.SaveChangesAsync();

        // Models
        var models = new List<Model>
        {
            new() { Name = "FX14-2", ManufacturerId = manufacturers[0].Id },
            new() { Name = "LFD2-24", ManufacturerId = manufacturers[1].Id },
            new() { Name = "OV-490A-2", ManufacturerId = manufacturers[2].Id },
            new() { Name = "LFD1-42", ManufacturerId = manufacturers[1].Id },
            new() { Name = "29", ManufacturerId = manufacturers[3].Id },
            new() { Name = "BK1104", ManufacturerId = manufacturers[4].Id },
            new() { Name = "DXU-1002-10", ManufacturerId = manufacturers[5].Id },
            new() { Name = "X2E-480", ManufacturerId = manufacturers[6].Id },
            new() { Name = "MCU-580SE", ManufacturerId = manufacturers[5].Id },
            new() { Name = "X2M-600", ManufacturerId = manufacturers[6].Id },
            new() { Name = "Shelf", ManufacturerId = manufacturers[7].Id }
        };
        await context.Models.AddRangeAsync(models);
        await context.SaveChangesAsync();

        // Boxes (Ovens, Dry Boxes, etc.)
        var ovenType = boxTypes.First(t => t.Name == "Oven");
        var dryBoxType = boxTypes.First(t => t.Name == "Dry Box");
        var cartType = boxTypes.First(t => t.Name == "Cart");

        var boxes = new List<Box>
        {
            new() { ToolId = "E02908", ModelId = models[0].Id, BoxTypeId = ovenType.Id, LocationId = locations[0].Id, DefaultTemperature = 80, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "E03010", ModelId = models[0].Id, BoxTypeId = ovenType.Id, LocationId = locations[0].Id, DefaultTemperature = 105, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "E03028", ModelId = models[0].Id, BoxTypeId = ovenType.Id, LocationId = locations[1].Id, DefaultTemperature = 105, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "E02909", ModelId = models[0].Id, BoxTypeId = ovenType.Id, LocationId = locations[1].Id, DefaultTemperature = 105, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "796296", ModelId = models[1].Id, BoxTypeId = ovenType.Id, LocationId = locations[0].Id, DefaultTemperature = 105, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "E03416", ModelId = models[2].Id, BoxTypeId = ovenType.Id, LocationId = locations[0].Id, DefaultTemperature = 77, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "110945", ModelId = models[2].Id, BoxTypeId = ovenType.Id, LocationId = locations[0].Id, DefaultTemperature = 105, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "670252", ModelId = models[3].Id, BoxTypeId = ovenType.Id, LocationId = locations[1].Id, DefaultTemperature = 65, TemperatureScale = "C", WarmUpTime = 10, HasDigitalDisplay = false },
            new() { ToolId = "800607", ModelId = models[4].Id, BoxTypeId = ovenType.Id, LocationId = locations[0].Id, DefaultTemperature = 65, TemperatureScale = "C", WarmUpTime = 210, HasDigitalDisplay = false },
            new() { ToolId = "81167", ModelId = models[5].Id, BoxTypeId = ovenType.Id, LocationId = locations[1].Id, DefaultTemperature = 65, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "K68406", ModelId = models[5].Id, BoxTypeId = ovenType.Id, LocationId = locations[2].Id, DefaultTemperature = 225, TemperatureScale = "F", HasDigitalDisplay = true },
            new() { ToolId = "702", ModelId = models[6].Id, BoxTypeId = dryBoxType.Id, LocationId = locations[0].Id, DefaultTemperature = 22, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "654", ModelId = models[6].Id, BoxTypeId = dryBoxType.Id, LocationId = locations[0].Id, DefaultTemperature = 22, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "E03328", ModelId = models[7].Id, BoxTypeId = dryBoxType.Id, LocationId = locations[1].Id, DefaultTemperature = 22, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "842", ModelId = models[6].Id, BoxTypeId = dryBoxType.Id, LocationId = locations[0].Id, DefaultTemperature = 22, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "456", ModelId = models[8].Id, BoxTypeId = dryBoxType.Id, LocationId = locations[1].Id, DefaultTemperature = 22, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "7682", ModelId = models[9].Id, BoxTypeId = dryBoxType.Id, LocationId = locations[2].Id, DefaultTemperature = 22, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "904", ModelId = models[10].Id, BoxTypeId = cartType.Id, LocationId = locations[5].Id, DefaultTemperature = 22, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "Cart 905", ModelId = models[10].Id, BoxTypeId = cartType.Id, LocationId = locations[5].Id, DefaultTemperature = 22, TemperatureScale = "C", HasDigitalDisplay = true },
            new() { ToolId = "Cart 906", ModelId = models[10].Id, BoxTypeId = cartType.Id, LocationId = locations[5].Id, DefaultTemperature = 22, TemperatureScale = "C", HasDigitalDisplay = true }
        };
        await context.Boxes.AddRangeAsync(boxes);
        await context.SaveChangesAsync();

        // Applications
        var applications = new List<Application>
        {
            new() { Name = "Ablefilm", DefaultBakeTime = 5.0, MinTemperature = 102, MaxTemperature = 108, Barcode = "*ABLEFILM*" },
            new() { Name = "Air Cure 2", DefaultBakeTime = 2.0, MinTemperature = 20, MaxTemperature = 25, Barcode = "*AIRCURE2*" },
            new() { Name = "Air Cure 24", DefaultBakeTime = 24.0, MinTemperature = 20, MaxTemperature = 25, Barcode = "*AIRCURE24*" },
            new() { Name = "Air Cure 48", DefaultBakeTime = 48.0, MinTemperature = 20, MaxTemperature = 25, Barcode = "*AIRCURE48*" },
            new() { Name = "Air Cure 8", DefaultBakeTime = 8.0, MinTemperature = 20, MaxTemperature = 25, Barcode = "*AIRCURE8*" },
            new() { Name = "Arathane 2", DefaultBakeTime = 2.0, MinTemperature = 74, MaxTemperature = 80, Barcode = "*ARATHANE2*" },
            new() { Name = "Arathane 8", DefaultBakeTime = 8.0, MinTemperature = 74, MaxTemperature = 80, Barcode = "*ARATHANE8*" },
            new() { Name = "Armstrong C7", DefaultBakeTime = 2.0, MinTemperature = 77, MaxTemperature = 83, Barcode = "*ARMSTRONGC7*" },
            new() { Name = "C7 CAB-O-SIL", DefaultBakeTime = 2.0, MinTemperature = 77, MaxTemperature = 83, Barcode = "*C7CABOSIL*" },
            new() { Name = "Chipbond 3607", DefaultBakeTime = 0.33, DefaultTemperature = 105, Barcode = "*CHIPBOND3607*" },
            new() { Name = "Chipbond 3621", DefaultBakeTime = 2.0, MinTemperature = 102, MaxTemperature = 108, Barcode = "*CHIPBOND3621*" },
            new() { Name = "Conap", DefaultBakeTime = 4.0, DefaultTemperature = 105, Barcode = "*CONAP*" },
            new() { Name = "Dymax", DefaultBakeTime = 0.5, DefaultTemperature = 105, Barcode = "*DYMAX*" },
            new() { Name = "Eccobond", DefaultBakeTime = 2.0, DefaultTemperature = 65, Barcode = "*ECCOBOND*" },
            new() { Name = "Enthone Ink", DefaultBakeTime = 1.0, DefaultTemperature = 105, Barcode = "*ENTHONEINK*" },
            new() { Name = "Humiseal 1B31", DefaultBakeTime = 0.5, DefaultTemperature = 77, Barcode = "*HUMISEAL1B31*" },
            new() { Name = "Humiseal 2A64", DefaultBakeTime = 3.0, DefaultTemperature = 77, Barcode = "*HUMISEAL2A64*" },
            new() { Name = "Hysol", DefaultBakeTime = 0.5, DefaultTemperature = 105, Barcode = "*HYSOL*" },
            new() { Name = "Label", DefaultBakeTime = 0.25, MinTemperature = 100, MaxTemperature = 110, Barcode = "*LABEL*" },
            new() { Name = "Lubricant", DefaultBakeTime = 0.5, DefaultTemperature = 65, Barcode = "*LUBRICANT*" },
            new() { Name = "Mask/Prebake", DefaultBakeTime = 4.0, DefaultTemperature = 105, Barcode = "*MASKPREBAKE*" },
            new() { Name = "Prebake", DefaultBakeTime = 0.5, DefaultTemperature = 105, Barcode = "*PREBAKE*" },
            new() { Name = "Resiweld", DefaultBakeTime = 1.0, MinTemperature = 65, MaxTemperature = 105, Barcode = "*RESIWELD*" },
            new() { Name = "RTV", DefaultBakeTime = 4.0, DefaultTemperature = 105, Barcode = "*RTV*" },
            new() { Name = "Scotchweld", DefaultBakeTime = 2.0, MinTemperature = 62, MaxTemperature = 68, Barcode = "*SCOTCHWELD*" },
            new() { Name = "Solder Mask", DefaultBakeTime = 0.5, DefaultTemperature = 105, Barcode = "*SOLDERMASK*" }
        };
        await context.Applications.AddRangeAsync(applications);
        await context.SaveChangesAsync();

        // Standard Times
        var standardTimes = new List<StandardTime>
        {
            new() { Hours = 0.05, Description = "0:03", Barcode = "*T003*" },
            new() { Hours = 0.167, Description = "0:10", Barcode = "*T010*" },
            new() { Hours = 0.25, Description = "0:15", Barcode = "*T015*" },
            new() { Hours = 0.5, Description = "0:30", Barcode = "*T030*" },
            new() { Hours = 1.0, Description = "1:00", Barcode = "*T100*" },
            new() { Hours = 1.5, Description = "1:30", Barcode = "*T130*" },
            new() { Hours = 2.0, Description = "2:00", Barcode = "*T200*" },
            new() { Hours = 3.0, Description = "3:00", Barcode = "*T300*" },
            new() { Hours = 3.5, Description = "3:30", Barcode = "*T330*" },
            new() { Hours = 4.0, Description = "4:00", Barcode = "*T400*" },
            new() { Hours = 4.5, Description = "4:30", Barcode = "*T430*" },
            new() { Hours = 5.0, Description = "5:00", Barcode = "*T500*" },
            new() { Hours = 5.5, Description = "5:30", Barcode = "*T530*" },
            new() { Hours = 6.0, Description = "6:00", Barcode = "*T600*" },
            new() { Hours = 7.0, Description = "7:00", Barcode = "*T700*" },
            new() { Hours = 7.5, Description = "7:30", Barcode = "*T730*" },
            new() { Hours = 8.0, Description = "8:00", Barcode = "*T800*" },
            new() { Hours = 12.0, Description = "12:00", Barcode = "*T1200*" },
            new() { Hours = 24.0, Description = "24:00", Barcode = "*T2400*" },
            new() { Hours = 48.0, Description = "48:00", Barcode = "*T4800*" },
            new() { Hours = 72.0, Description = "72:00", Barcode = "*T7200*" }
        };
        await context.StandardTimes.AddRangeAsync(standardTimes);
        await context.SaveChangesAsync();

        // Parts
        var parts = new List<Part>
        {
            new() { PartNumber = "PN001", Description = "Test Part 1" },
            new() { PartNumber = "PN002", Description = "Test Part 2" },
            new() { PartNumber = "PN003", Description = "Test Part 3" }
        };
        await context.Parts.AddRangeAsync(parts);
        await context.SaveChangesAsync();

        // TRAKs for testing
        var traks = new List<Trak>();
        for (int i = 1; i <= 63; i++)
        {
            traks.Add(new Trak
            {
                TrakId = $"TRK{i:D2}",
                SerialNumber = $"SN{i:D2}",
                WorkOrder = $"WO{(i % 10) + 1:D3}",
                PartId = parts[i % 3].Id
            });
        }
        await context.Traks.AddRangeAsync(traks);
        await context.SaveChangesAsync();

        // Users
        var users = new List<User>
        {
            new() { FirstName = "John", LastName = "Doe", Badge = "12345", Login = "john.doe", IsActive = true },
            new() { FirstName = "Jane", LastName = "Smith", Badge = "12346", Login = "jane.smith", IsActive = true },
            new() { FirstName = "Bob", MiddleName = "M", LastName = "Johnson", Badge = "12347", Login = "bob.m.johnson", IsActive = true },
            new() { FirstName = "Alice", LastName = "Williams", Badge = "12348", Login = "alice.williams", IsActive = true },
            new() { FirstName = "Test", LastName = "User", Badge = "00000", Login = "test.user", IsActive = true }
        };
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        // Add some sample events (TRAKs in ovens)
        var sampleEvents = new List<OvenEvent>
        {
            new()
            {
                BoxId = boxes[8].Id, // 800607
                TrakId = traks[1].Id, // TRK02
                UserInId = users[0].Id,
                DateIn = DateTime.UtcNow.AddHours(-1),
                Temperature = 65,
                BakeTimeHours = 2.0,
                Quantity = 1,
                ApplicationId = applications[13].Id, // Eccobond
                Note = "Sample event"
            }
        };
        await context.OvenEvents.AddRangeAsync(sampleEvents);
        await context.SaveChangesAsync();
    }
}

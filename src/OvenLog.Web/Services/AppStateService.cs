using OvenLog.Domain.Entities;

namespace OvenLog.Web.Services;

public class AppStateService
{
    private User? _currentUser;
    private List<Trak> _selectedTraks = new();
    private List<OvenEvent> _selectedOvenEvents = new();
    private Box? _selectedBox;
    private Application? _selectedApplication;
    private double _temperature;
    private double _bakeTimeHours;
    private int _quantity = 1;
    private DateTime _startTime = DateTime.Now;
    private string _note = string.Empty;

    public User? CurrentUser
    {
        get => _currentUser;
        set
        {
            _currentUser = value;
            OnStateChanged?.Invoke();
        }
    }

    public List<Trak> SelectedTraks
    {
        get => _selectedTraks;
        set
        {
            _selectedTraks = value;
            OnStateChanged?.Invoke();
        }
    }

    public List<OvenEvent> SelectedOvenEvents
    {
        get => _selectedOvenEvents;
        set
        {
            _selectedOvenEvents = value;
            OnStateChanged?.Invoke();
        }
    }

    public Box? SelectedBox
    {
        get => _selectedBox;
        set
        {
            _selectedBox = value;
            if (value != null)
            {
                _temperature = value.DefaultTemperature;
            }
            OnStateChanged?.Invoke();
        }
    }

    public Application? SelectedApplication
    {
        get => _selectedApplication;
        set
        {
            _selectedApplication = value;
            if (value?.DefaultBakeTime.HasValue == true)
            {
                _bakeTimeHours = value.DefaultBakeTime.Value;
            }
            if (value?.DefaultTemperature.HasValue == true)
            {
                _temperature = value.DefaultTemperature.Value;
            }
            OnStateChanged?.Invoke();
        }
    }

    public double Temperature
    {
        get => _temperature;
        set
        {
            _temperature = value;
            OnStateChanged?.Invoke();
        }
    }

    public double BakeTimeHours
    {
        get => _bakeTimeHours;
        set
        {
            _bakeTimeHours = value;
            OnStateChanged?.Invoke();
        }
    }

    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            OnStateChanged?.Invoke();
        }
    }

    public DateTime StartTime
    {
        get => _startTime;
        set
        {
            _startTime = value;
            OnStateChanged?.Invoke();
        }
    }

    public string Note
    {
        get => _note;
        set
        {
            _note = value;
            OnStateChanged?.Invoke();
        }
    }

    public event Action? OnStateChanged;

    public void Reset()
    {
        _selectedTraks.Clear();
        _selectedOvenEvents.Clear();
        _selectedBox = null;
        _selectedApplication = null;
        _temperature = 0;
        _bakeTimeHours = 0;
        _quantity = 1;
        _startTime = DateTime.Now;
        _note = string.Empty;
        OnStateChanged?.Invoke();
    }

    public void ClearTrakSelection()
    {
        _selectedTraks.Clear();
        OnStateChanged?.Invoke();
    }

    public void ClearOvenEventSelection()
    {
        _selectedOvenEvents.Clear();
        OnStateChanged?.Invoke();
    }

    public void AddTrak(Trak trak)
    {
        if (!_selectedTraks.Any(t => t.Id == trak.Id))
        {
            _selectedTraks.Add(trak);
            OnStateChanged?.Invoke();
        }
    }

    public void RemoveTrak(Trak trak)
    {
        _selectedTraks.RemoveAll(t => t.Id == trak.Id);
        OnStateChanged?.Invoke();
    }

    public void ToggleTrak(Trak trak)
    {
        if (_selectedTraks.Any(t => t.Id == trak.Id))
            RemoveTrak(trak);
        else
            AddTrak(trak);
    }

    public void AddOvenEvent(OvenEvent ovenEvent)
    {
        if (!_selectedOvenEvents.Any(e => e.Id == ovenEvent.Id))
        {
            _selectedOvenEvents.Add(ovenEvent);
            OnStateChanged?.Invoke();
        }
    }

    public void RemoveOvenEvent(OvenEvent ovenEvent)
    {
        _selectedOvenEvents.RemoveAll(e => e.Id == ovenEvent.Id);
        OnStateChanged?.Invoke();
    }

    public void ToggleOvenEvent(OvenEvent ovenEvent)
    {
        if (_selectedOvenEvents.Any(e => e.Id == ovenEvent.Id))
            RemoveOvenEvent(ovenEvent);
        else
            AddOvenEvent(ovenEvent);
    }

    public void SelectAllTraks(IEnumerable<Trak> traks)
    {
        _selectedTraks = traks.ToList();
        OnStateChanged?.Invoke();
    }

    public void DeselectAllTraks()
    {
        _selectedTraks.Clear();
        OnStateChanged?.Invoke();
    }

    public bool CanAdd()
    {
        return _selectedTraks.Any() &&
               _selectedBox != null &&
               _temperature > 0 &&
               _bakeTimeHours > 0 &&
               _quantity > 0 &&
               _currentUser != null;
    }

    public bool CanRemove()
    {
        return _selectedOvenEvents.Any() && _currentUser != null;
    }
}

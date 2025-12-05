namespace OvenLog.Web.Services;

public enum BarcodeType
{
    Unknown,
    Trak,
    Oven,
    Application,
    StandardTime,
    ActionReset,
    ActionOvenOn,
    ActionAdd,
    ActionRemove
}

public class BarcodeScannedEventArgs : EventArgs
{
    public string Barcode { get; }
    public BarcodeType Type { get; }

    public BarcodeScannedEventArgs(string barcode, BarcodeType type)
    {
        Barcode = barcode;
        Type = type;
    }
}

public class BarcodeInputService
{
    private bool _isBarcodeMode = true;
    private string _buffer = string.Empty;
    private DateTime _lastKeyTime = DateTime.MinValue;
    private readonly TimeSpan _scanTimeout = TimeSpan.FromMilliseconds(100);

    // Known patterns for routing
    private HashSet<string> _knownOvenIds = new();
    private HashSet<string> _knownApplicationBarcodes = new();
    private HashSet<string> _knownTimeBarcodes = new();

    public bool IsBarcodeMode => _isBarcodeMode;
    
    public event EventHandler<BarcodeScannedEventArgs>? OnBarcodeScanned;
    public event EventHandler? OnModeChanged;

    public void SetBarcodeMode(bool enabled)
    {
        _isBarcodeMode = enabled;
        _buffer = string.Empty;
        OnModeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ToggleMode()
    {
        SetBarcodeMode(!_isBarcodeMode);
    }

    public void RegisterOvenIds(IEnumerable<string> ovenIds)
    {
        _knownOvenIds = new HashSet<string>(ovenIds, StringComparer.OrdinalIgnoreCase);
    }

    public void RegisterApplicationBarcodes(IEnumerable<string> barcodes)
    {
        _knownApplicationBarcodes = new HashSet<string>(barcodes.Where(b => !string.IsNullOrEmpty(b)), StringComparer.OrdinalIgnoreCase);
    }

    public void RegisterTimeBarcodes(IEnumerable<string> barcodes)
    {
        _knownTimeBarcodes = new HashSet<string>(barcodes.Where(b => !string.IsNullOrEmpty(b)), StringComparer.OrdinalIgnoreCase);
    }

    public void HandleKeyDown(string key)
    {
        if (!_isBarcodeMode)
            return;

        var now = DateTime.Now;
        
        // Check if this is a new scan (timeout exceeded)
        if ((now - _lastKeyTime) > _scanTimeout && !string.IsNullOrEmpty(_buffer))
        {
            // Previous buffer was incomplete, clear it
            _buffer = string.Empty;
        }
        
        _lastKeyTime = now;

        // Handle terminator keys
        if (key == "Tab" || key == "Enter")
        {
            if (!string.IsNullOrEmpty(_buffer))
            {
                ProcessBarcode(_buffer);
                _buffer = string.Empty;
            }
            return;
        }

        // Add character to buffer
        if (key.Length == 1)
        {
            _buffer += key;
        }
    }

    private void ProcessBarcode(string barcode)
    {
        var type = DetermineType(barcode);
        OnBarcodeScanned?.Invoke(this, new BarcodeScannedEventArgs(barcode, type));
    }

    private BarcodeType DetermineType(string barcode)
    {
        // Check for action barcodes
        var upperBarcode = barcode.ToUpperInvariant();
        if (upperBarcode.Contains("RESET") || upperBarcode == "*RESET*")
            return BarcodeType.ActionReset;
        if (upperBarcode.Contains("OVENON") || upperBarcode == "*OVENON*")
            return BarcodeType.ActionOvenOn;
        if (upperBarcode.Contains("ADD") || upperBarcode == "*ADD*")
            return BarcodeType.ActionAdd;
        if (upperBarcode.Contains("REMOVE") || upperBarcode == "*REMOVE*")
            return BarcodeType.ActionRemove;

        // Check for known oven IDs
        if (_knownOvenIds.Contains(barcode))
            return BarcodeType.Oven;

        // Check for known application barcodes
        if (_knownApplicationBarcodes.Contains(barcode))
            return BarcodeType.Application;

        // Check for known time barcodes
        if (_knownTimeBarcodes.Contains(barcode))
            return BarcodeType.StandardTime;

        // Check for TRAK pattern (starts with TRK or similar)
        if (barcode.StartsWith("TRK", StringComparison.OrdinalIgnoreCase) ||
            barcode.StartsWith("TRAK", StringComparison.OrdinalIgnoreCase))
            return BarcodeType.Trak;

        // Default to TRAK for unknown barcodes (most common scan)
        return BarcodeType.Trak;
    }

    public void Reset()
    {
        _buffer = string.Empty;
        SetBarcodeMode(true);
    }
}

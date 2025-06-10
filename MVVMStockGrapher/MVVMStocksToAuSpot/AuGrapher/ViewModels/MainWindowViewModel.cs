namespace AuGrapher.ViewModels;

using System;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.Input;
using AuGrapher.StockPriceToDb;
using AuGrapher.DbReader;
using System.Linq;

public partial class MainWindowViewModel : ViewModelBase
{
    private string _ticker = "";
    public string Ticker
    {
        get => _ticker;
        set
        {
            if (_ticker != value)
            {
                _ticker = value;
                OnPropertyChanged();
            }
        }
    }

    private ISeries[] _series = Array.Empty<ISeries>();
    public ISeries[] Series
    {
        get => _series;
        set
        {
            _series = value;
            OnPropertyChanged();
        }
    }
    private Axis[] _xAxis = Array.Empty<Axis>();
    public Axis[] XAxis
    {
        get => _xAxis;
        set
        {
            _xAxis = value;
            OnPropertyChanged();
        }
    }
    private Axis[] _yAxis = Array.Empty<Axis>();
    public Axis[] YAxis
    {
        get => _yAxis;
        set
        {
            _yAxis = value;
            OnPropertyChanged();
        }
    }
    private bool _hasChart;
    public bool HasChart
    {
        get => _hasChart;
        set
        {
            _hasChart = value;
            OnPropertyChanged();
        }
    }

    public ICommand GraphCommand { get; }

    public MainWindowViewModel()
    {
        GraphCommand = new RelayCommand(LoadChartData);
    }

    private void LoadChartData()
    {
        //check if ticker is in db and up to date
        if (!DbReader.TickerInDb(Ticker) || !DbReader.TickerDataUpToDate(Ticker))
        {
            // Insert to DB if not in Db
            StockPriceToDb.InsertCsvDataToDb(Ticker);
        }         

        var data = DbReader.GetAdjustedStockHistory(Ticker);

        var points = data
            .Where(d => d.Item2.HasValue)
            .Select(d => new ObservablePoint(d.Item1.ToOADate(), d.Item2!.Value))
            .ToArray();

        Series = new ISeries[]
        {
            new LineSeries<ObservablePoint>
            {
                Values = points,
                GeometrySize = 8,
                Fill = null
            }
        };

        XAxis = new Axis[]
        {
            new Axis
            {
                Labeler = value => DateTime.FromOADate(value).ToString("MMM yyyy"),
                UnitWidth = TimeSpan.FromDays(30).TotalDays,
                LabelsRotation = 45
            }
        };

        YAxis = new Axis[] { new Axis() };

        HasChart = true;
    }
}

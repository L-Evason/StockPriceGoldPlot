using Avalonia.Controls;
using AuGrapher.ViewModels;
namespace AuGrapher.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
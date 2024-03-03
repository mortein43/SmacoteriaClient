using Smacoteria.Model;
using Smacoteria.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Smacoteria.View;


public partial class DishDetailWindow : Window
{
    private Point startPoint;
    private ViewModelMainWindow viewModelMainWindow;

    public DishDetailWindow(object selectedDish, ObservableCollection<Addition> additionsFinal, ViewModelMainWindow mainWindowViewModel)
    {
        this.InitializeComponent();
        this.DataContext = new ViewModelDishDetail(selectedDish, additionsFinal, mainWindowViewModel);
        this.viewModelMainWindow = mainWindowViewModel;
    }

    private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.startPoint = e.GetPosition(null);
        }
    }

    private void ScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        ScrollViewer scrollViewer = sender as ScrollViewer;
        if (scrollViewer != null && e.LeftButton == MouseButtonState.Pressed)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = this.startPoint - mousePos;

            if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance)
            {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + diff.X);
            }

            this.startPoint = mousePos;
        }
    }

    private void ScrollViewerVertical_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        ScrollViewer scrollViewer = sender as ScrollViewer;
        if (scrollViewer != null && e.LeftButton == MouseButtonState.Pressed)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = this.startPoint - mousePos;

            if (Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + diff.Y);
            }

            this.startPoint = mousePos;
        }
    }

    private void AdditionButton_Click(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;

        if (button != null && button.DataContext != null)
        {
            var dataContext = button.DataContext;

            if (dataContext != null && this.DataContext is ViewModelDishDetail viewModel)
            {
                Addition addition = (Addition)dataContext;
                viewModel.ExecuteAdditionClick(addition);
            }
        }
    }

    private void AdditionDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;

        if (button != null && button.DataContext != null)
        {
            var dataContext = button.DataContext;

            if (dataContext != null && this.DataContext is ViewModelDishDetail viewModel)
            {
                Addition addition = (Addition)dataContext;
                viewModel.ExecuteAdditionDelete(addition);
            }
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Window window = Window.GetWindow(this);
        window?.Close();
    }
}

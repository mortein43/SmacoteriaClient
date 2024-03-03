using Smacoteria.Model;
using Smacoteria.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Smacoteria;


public partial class MainWindow : Window
{
    private Point startPoint;

    public MainWindow()
    {
        this.InitializeComponent();
        this.Closing += this.MainWindow_Closing;
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        if (this.DataContext != null && this.DataContext is ViewModelMainWindow viewModel)
            viewModel.tcpControl.Disconnect();
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

            if (Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + diff.Y);
            }

            this.startPoint = mousePos;
        }
    }

    private void ScrollViewerHorisontal_PreviewMouseMove(object sender, MouseEventArgs e)
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

    private void DishInOrderDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;

        if (button != null && button.DataContext != null)
        {
            var dataContext = button.DataContext;

            if (dataContext != null && this.DataContext is ViewModelMainWindow viewModel)
            {
                DishInOrder dishInOrder = (DishInOrder)dataContext;
                viewModel.ExecuteDishInOrderDelete(dishInOrder);
            }
        }
    }

    private void DishButton_Click(object sender, RoutedEventArgs e)
    {
        Border border = sender as Border;

        if (border != null && border.DataContext != null)
        {
            var dataContext = border.DataContext;

            if (dataContext != null && this.DataContext is ViewModelMainWindow viewModel)
            {
                Dish dish = (Dish)dataContext;
                viewModel.ExecuteDishClick(dish);
            }
        }
    }
}
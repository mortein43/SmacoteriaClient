using Smacoteria.Model;
using Smacoteria.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Smacoteria.View
{
    public partial class SendOrder : Window
    {
        private ViewModelMainWindow viewModelMainWindow;
        private Point startPoint;

        public SendOrder(ViewModelMainWindow viewModelMainWindow)
        {
            this.InitializeComponent();
            this.DataContext = new ViewModelSendOrder(viewModelMainWindow);
            this.viewModelMainWindow = viewModelMainWindow;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null && this.DataContext is ViewModelSendOrder viewModelSendOrder)
            {
                viewModelSendOrder.ExecuteSendOrder(this.NoticeInput.Text);
            }

            Window window = Window.GetWindow(this);
            window?.Close();
        }

        private void ChangePayment(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == this.CashButton)
            {
                this.CashButton.Background = Brushes.LightGray;
                this.WithoutCashButton.Background = Brushes.White;
            }
            else
            {
                this.CashButton.Background = Brushes.White;
                this.WithoutCashButton.Background = Brushes.LightGray;
            }
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
    }
}

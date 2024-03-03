namespace Smacoteria.ViewModel;
using GalaSoft.MvvmLight.Command;
using Smacoteria.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

public class ViewModelSendOrder
{
    public Order DishesInPayment { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ICommand Cash { get; set; }

    public ICommand WithoutСash { get; set; }

    public ViewModelMainWindow _mainWindowViewModel { get; set; }

    public ViewModelSendOrder(ViewModelMainWindow _mainWindowViewModel)
    {
        this._mainWindowViewModel = _mainWindowViewModel;
        this.DishesInPayment = _mainWindowViewModel.Order;
        this.Cash = new RelayCommand(this.ExecuteCash, this.CanExecuteCash);
        this.WithoutСash = new RelayCommand(this.ExecuteWithoutСash, this.CanExecuteWithoutСash);
    }

    private bool CanExecuteWithoutСash()
    {
        return true;
    }

    private void ExecuteWithoutСash()
    {
        this._mainWindowViewModel.order.PaymentMethod = Order.PaymentMethods.Картою;
    }

    private bool CanExecuteCash()
    {
        return true;
    }

    private void ExecuteCash()
    {
        this._mainWindowViewModel.order.PaymentMethod = Order.PaymentMethods.Готівкою;
    }

    public ViewModelSendOrder()
    {
    }

    public void ExecuteSendOrder(string notice)
    {
        if (this._mainWindowViewModel.order.PaymentMethod == null)
        {
            this._mainWindowViewModel.order.PaymentMethod = Order.PaymentMethods.Готівкою;
        }

        this._mainWindowViewModel.order.Notice = notice;
        this._mainWindowViewModel.order.Status = Order.Statuses.Очікується;
        this._mainWindowViewModel.tcpControl.SendOrderToServer(this._mainWindowViewModel.order);
        var tableNumber = this._mainWindowViewModel.order.TableNumber;
        this._mainWindowViewModel.order = new Order();
        this._mainWindowViewModel.DishesInOrder = new ObservableCollection<DishInOrder>();
        this._mainWindowViewModel.TotalCoast = 0;
        this._mainWindowViewModel.order.TableNumber = tableNumber;
    }
}

namespace Smacoteria.ViewModel;
using GalaSoft.MvvmLight.Command;
using Smacoteria.Controls;
using Smacoteria.Model;
using Smacoteria.View;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

public class ViewModelMainWindow : INotifyPropertyChanged
{
    public float _totalCoast;
    public Order order = new Order();
    public TCPControl tcpControl;

    public ObservableCollection<Dish>? _croissantSandwiches = new();
    public ObservableCollection<Dish>? _sweetCroissants = new();
    public ObservableCollection<Dish>? _drinks = new();
    private ObservableCollection<Dish>? _createown = new();
    private ObservableCollection<Dish>? _dishesFromShow = new();
    public ObservableCollection<Addition>? _additions = new();
    private ObservableCollection<DishInOrder>? _dishesInOrder = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand ShowSweetCroissants { get; }

    public ICommand ShowCreateOwn { get; }

    public ICommand ShowCroissantSandwiches { get; }

    public ICommand ShowDrinks { get; }

    public ICommand SendOrder { get; }

    public Order Order
    {
        get
        {
            return this.order;
        }

        set
        {
            this.order = value;
            this.OnPropertyChanged(nameof(this.Order));
        }
    }

    public float TotalCoast
    {
        get
        {
            return this._totalCoast;
        }

        set
        {
            this._totalCoast = value;
            this.OnPropertyChanged(nameof(this.TotalCoast));
        }
    }

    public ObservableCollection<Dish> DisheFromShow
    {
        get
        {
            return this._dishesFromShow;
        }

        set
        {
            this._dishesFromShow = value;
            this.OnPropertyChanged(nameof(this.DisheFromShow));
        }
    }

    public ObservableCollection<DishInOrder> DishesInOrder
    {
        get
        {
            return this._dishesInOrder;
        }

        set
        {
            this._dishesInOrder = value;
            this.OnPropertyChanged(nameof(this.DishesInOrder));
        }
    }

    public ObservableCollection<Dish> CroissantSandwiches
    {
        get
        {
            return this._croissantSandwiches;
        }

        set
        {
            this._croissantSandwiches = value;
            this.OnPropertyChanged(nameof(this.CroissantSandwiches));
        }
    }

    public ObservableCollection<Dish> SweetCroissants
    {
        get
        {
            return this._sweetCroissants;
        }

        set
        {
            this._sweetCroissants = value;
            this.OnPropertyChanged(nameof(this.SweetCroissants));
        }
    }

    public ObservableCollection<Dish> CreateOwn
    {
        get
        {
            return this._createown;
        }

        set
        {
            this._createown = value;
            this.OnPropertyChanged(nameof(this.CreateOwn));
        }
    }

    public ObservableCollection<Dish> Drinks
    {
        get
        {
            return this._drinks;
        }

        set
        {
            this._drinks = value;
            this.OnPropertyChanged(nameof(this.Drinks));
        }
    }

    public ObservableCollection<Addition> Additions
    {
        get
        {
            return this._additions;
        }

        set
        {
            this._additions = value;
            this.OnPropertyChanged(nameof(this.Additions));
        }
    }

    public ObservableCollection<Addition> AdditionsFinal
    {
        get
        {
            return this._additions;
        }

        set
        {
            this._additions = value;
            this.OnPropertyChanged(nameof(this.AdditionsFinal));
        }
    }

    public void ExecuteDishClick(object selectedDish)
    {
        Dish d = selectedDish as Dish;
        ObservableCollection<Addition> _additionsFinal = new ObservableCollection<Addition>(this.Additions.Where(a => a.CategoryId == d.CategoryId && d.Name != "Курячий бульйон"));
        DishDetailWindow dishDetailWindow = new DishDetailWindow(selectedDish, _additionsFinal, this);
        dishDetailWindow.ShowDialog();
    }

    public void ExecuteDishInOrderDelete(DishInOrder selectedDishInOrder)
    {
        this.DishesInOrder.Remove(selectedDishInOrder);
        this.order.Dishes.Remove(selectedDishInOrder);
        this.order.TotalCost -= selectedDishInOrder.TotalCoast;
        this.TotalCoast = this.order.TotalCost;
    }

    public ViewModelMainWindow()
    {
        this.tcpControl = new TCPControl(this);
        this.order.TableNumber = this.tcpControl.tableNumber;

        this.LoadDishes();
        this.ShowSweetCroissants = new RelayCommand(this.ExecuteShowSweetCroissants, this.CanExecuteShowSweetCroissants);
        this.ShowCroissantSandwiches = new RelayCommand(this.ExecuteShowCroissantSandwiches, this.CanExecuteShowCroissantSandwiches);
        this.ShowDrinks = new RelayCommand(this.ExecuteShowDrinks, this.CanExecuteShowDrinks);
        this.ShowCreateOwn = new RelayCommand(this.ExecuteShowCreateOwn, this.CanExecuteShowCreateOwn);
        this.SendOrder = new RelayCommand(this.ExecuteSendOrder, this.CanExecuteSendOrder);
    }

    private void LoadDishes()
    {
        ObservableCollection<Dish> allDishes = new ObservableCollection<Dish>(this.tcpControl.receivedListDishes);
        this.DisheFromShow = new ObservableCollection<Dish>(allDishes.Where(d => d.CategoryId == 1).ToList());
        this._croissantSandwiches = new ObservableCollection<Dish>(allDishes.Where(d => d.CategoryId == 1).ToList());
        this._sweetCroissants = new ObservableCollection<Dish>(allDishes.Where(d => d.CategoryId == 2).ToList());
        this._drinks = new ObservableCollection<Dish>(allDishes.Where(d => d.CategoryId == 3).ToList());
        this._createown = new ObservableCollection<Dish>(allDishes.Where(d => d.Name == "Круасан сендвіч" || d.Name == "Круасан солодкий").ToList());
        this._additions = new ObservableCollection<Addition>(this.tcpControl.receivedListAdditions);
        this.TotalCoast = 0;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool CanExecuteShowSweetCroissants()
    {
        return true;
    }

    private bool CanExecuteShowCroissantSandwiches()
    {
        return true;
    }

    private bool CanExecuteShowDrinks()
    {
        return true;
    }

    private bool CanExecuteShowCreateOwn()
    {
        return true;
    }

    private bool CanExecuteSendOrder()
    {
        return true;
    }

    private void ExecuteShowSweetCroissants()
    {
        this.DisheFromShow = this.SweetCroissants;
    }

    private void ExecuteShowCroissantSandwiches()
    {
        this.DisheFromShow = this.CroissantSandwiches;
    }

    private void ExecuteShowDrinks()
    {
        this.DisheFromShow = this.Drinks;
    }

    private void ExecuteShowCreateOwn()
    {
        this.DisheFromShow = this.CreateOwn;
    }

    private void ExecuteSendOrder()
    {
        if (this.order.Dishes.Count != 0)
        {
            SendOrder sendOrderlWindow = new SendOrder(this);
            sendOrderlWindow.ShowDialog();
        }
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        this.tcpControl.Disconnect();
    }
}
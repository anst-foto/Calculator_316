using System.Reactive;
using Calculator.Core;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Calculator.Desktop.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    private CancellationTokenSource _ctsCalculateSum;
    private CancellationTokenSource _ctsCalculateFactorial;

    private int? _numberStartNum = null;
    private long? _startSum = null;
    private int? _numberFactorial;
    
    [Reactive] public string? InputNumberText { get; set; }
    [Reactive] public string? OutputSumText { get; set; }
    [Reactive] public string? OutputFactorialText { get; set; }
    
    [Reactive] public double ProgressSumValue { get; set; }
    [Reactive] public double ProgressFactorialValue { get; set; }
    
    [Reactive] public string? StatusCalculateSum { get; set; }
    [Reactive] public string? StatusCalculateFactorial { get; set; }
    
    public ReactiveCommand<Unit, Unit> CommandCalculateSumStart { get; }
    public ReactiveCommand<Unit, Unit> CommandCalculateSumStop { get; }
    
    public ReactiveCommand<Unit, Unit> CommandCalculateFactorialStart { get; }
    public ReactiveCommand<Unit, Unit> CommandCalculateFactorialStop { get; }

    public MainWindowViewModel()
    {
        IObservable<bool> canExecuteCalculateSumStart = this.WhenAnyValue(
            p1 => p1.InputNumberText,
            p1 => !string.IsNullOrWhiteSpace(p1));

        var canExecuteCalculateSumStop = this.WhenAnyValue(
            p => p.ProgressSumValue,
            p => p > 0);

        CommandCalculateSumStart = ReactiveCommand.CreateFromTask(
            execute: async () =>
            {
                _ctsCalculateSum = new CancellationTokenSource();

                var startNum = _numberStartNum ?? 0;
                var endNum = int.Parse(InputNumberText!);
                var startSum = _startSum ?? 0;

                var progress = new Progress<(int index, long sum)>();
                progress.ProgressChanged += (_, report) =>
                {
                    ProgressSumValue = report.index;
                    
                    _startSum = report.sum;
                    _numberStartNum = report.index + 1;
                };

                try
                {
                    StatusCalculateSum = "Вычисление суммы...";
                    
                    var result = await Calc.SumAsync(
                        startNum: startNum,
                        endNumber: endNum,
                        startSum: startSum,
                        cancellationToken: _ctsCalculateSum.Token,
                        progress: progress);
                    
                    OutputSumText = result.ToString();
                    
                    _numberStartNum = null;
                    _startSum = null;
                    
                    StatusCalculateSum = "Вычисление суммы завершено";
                }
                catch (Exception)
                {
                    StatusCalculateSum = "Приостановка вычисления суммы";
                }
            },
            canExecute: canExecuteCalculateSumStart);
        
        CommandCalculateSumStop = ReactiveCommand.CreateFromTask(
            execute: async () =>
            {
                await _ctsCalculateSum!.CancelAsync();
            },
            canExecute: canExecuteCalculateSumStop);
    }
}
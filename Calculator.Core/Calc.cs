namespace Calculator.Core;

public static class Calc
{
    public static async Task<long> FactorialAsync(int number, CancellationToken cancellationToken = default,
        IProgress<int>? progress = null)
    {
        long result = 1;
        for (int i = number; i >= 1;  i--)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            progress?.Report(number - i);
            
            await Task.Delay(100, cancellationToken);
            
            result *= i;
        }
        
        return result;
    }

    public static async Task<long> SumAsync(int startNum, int endNumber, long startSum, CancellationToken cancellationToken = default,
        IProgress<(int, long)>? progress = null)
    {
        long result = startSum;
        
        for (int i = startNum; i <= endNumber; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            result += i;
            
            progress?.Report((i, result));
            
            await Task.Delay(100, cancellationToken);
        }

        return result;
    }
}
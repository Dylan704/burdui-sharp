namespace CSharpRecap;

public class Counter
{
    
    // C# properties are instance variables incapsulated with getters and setters
    // here the counter value can be read from outside the class but writing
    // is limited to the Counter class and sub-classes. 
    public int CounterValue { get; internal set; }

    // Delegates represent a signature of an instance method. In this case
    // we are representing methods 
    // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/

    public delegate int CounterChangedDelegate(Counter source);
    
    // Events are language-level implementations of the observer pattern. 
    // It provides: 
    // 1) the listener list (of delegates) and methods to manage 
    // 2) the event notification mechanism (i.e., invoking all delegates in the list)
    // NB the question mark indicates that the variable can be null. 
    public event CounterChangedDelegate? CounterChanged;
    
    public Counter()
    {
        // dummy instruction
        var t = new Object();
        
        // we use properties as variables
        CounterValue = 0;
    }

    public void AddOne()
    {
        CounterValue++;
        
        // this raises the event (notifies observers of the counter value change)
        // the question mark avoids invoking the method if the event is null 
        // (i.e., no one subscribed it)
        this.CounterChanged?.Invoke(this);
    }
    
}
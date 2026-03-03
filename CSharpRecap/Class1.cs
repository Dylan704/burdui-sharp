namespace CSharpRecap;

public class Class1
{
    public void TestMethod1()
    {
        // creating a new Counter
        var cl2 = new Counter();
        

        // registering two listeners to the CounterChanged event
        // 1) anonymous method
        cl2.CounterChanged += source =>
        {
            Console.WriteLine("Changed " + cl2.CounterValue);
            return cl2.CounterValue;
        };

        // 2) passing a method with a name
        // NB += is the syntax for registering to events. It's an operator overload. 
        cl2.CounterChanged += CallMyDelegate;
        
        cl2.AddOne();
    }

    public int CallMyDelegate(Counter cl2)
    {
        return 0;
    }
}
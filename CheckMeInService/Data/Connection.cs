namespace CheckMeInService.Data;

public abstract class Connection 
{
    // Since the connection string is the only thing that stays the same across classes then it is the only thing that needs to be in the base class. As we don't have anything else concrete
    protected string _connectionString;
    
    // Can use polymorphism only to create different implementations of a single method for a class that is dervied from this class 
}

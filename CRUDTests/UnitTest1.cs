namespace CRUDTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        //Arrange - declaration of variables
        MyMath mm =new();
        int input1 = 10, input2 = 5;
        int expected = 15;

        //Act -calling the method
        int actual = mm.Add(input1, input2);

        //Assert - compering expected value with actual value
        Assert.Equal(expected, actual);
        
    }
}
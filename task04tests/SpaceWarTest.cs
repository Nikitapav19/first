using Xunit;
using Moq;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }

    [Fact]
    public void Fighter_ShouldBeFasterThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed > cruiser.Speed);
    }
    
    [Fact]
    public void Fighter_ShouldHaveCorrectStats()
    {
        var fighter = new Fighter();
        Assert.Equal(20, fighter.FirePower);
        Assert.Equal(100, fighter.Speed);
    }

    [Fact]
    public void Cruiser_ShouldBeStrongerThanFighter()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.FirePower < cruiser.FirePower);
    }

    [Fact]
    public void Fighter_ShouldRotateCorrectly()
    {
        var fighter = new Fighter();
        fighter.Rotate(100);
        fighter.Rotate(20);
        Assert.Equal(120, fighter.CurAngle);
    }

    [Fact]
    public void Fighter_ShouldRotateCorrectlyTooBigAngle()
    {
        var fighter = new Fighter();
        fighter.Rotate(300);
        fighter.Rotate(100);
        Assert.Equal(40, fighter.CurAngle);
    }

    [Fact]
    public void Fighter_ShouldMoveCorrectly()
    {
        var fighter = new Fighter();
        fighter.MoveForward();
        fighter.MoveForward();
        Assert.Equal(200, fighter.Position);
    }

    [Fact]
    public void Fighter_ShouldShootCorrectly()
    {
        var fighter = new Fighter();
        fighter.Fire();
        fighter.Fire();
        fighter.Fire();
        Assert.Equal(47, fighter.Ammo);
    }


    [Fact]
    public void Cruiser_ShouldRotateCorrectly()
    {
        var cruiser = new Cruiser();
        cruiser.Rotate(100);
        cruiser.Rotate(20);
        Assert.Equal(120, cruiser.CurAngle);
    }

    [Fact]
    public void Cruiser_ShouldRotateCorrectlyTooBigAngle()
    {
        var cruiser = new Cruiser();
        cruiser.Rotate(300);
        cruiser.Rotate(100);
        Assert.Equal(40, cruiser.CurAngle);
    }

    [Fact]
    public void Cruiser_ShouldMoveCorrectly()
    {
        var cruiser = new Cruiser();
        cruiser.MoveForward();
        cruiser.MoveForward();
        Assert.Equal(100, cruiser.Position);
    }

    [Fact]
    public void Cruiser_ShouldShootCorrectly()
    {
        var cruiser = new Cruiser();
        cruiser.Fire();
        cruiser.Fire();
        cruiser.Fire();
        Assert.Equal(7, cruiser.Ammo);
    }
}   
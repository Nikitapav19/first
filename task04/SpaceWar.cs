public interface ISpaceship
{
    void MoveForward();      // Движение вперед
    void Rotate(int angle);  // Поворот на угол (градусы)
    void Fire();             // Выстрел ракетой
    int Speed { get; }       // Скорость корабля
    int FirePower { get; }   // Мощность выстрела
}

public class Cruiser : ISpaceship
{
    public int Position { get ; private set ; } = 0;
    public int Ammo { get ; private set ; } = 10;
    public int CurAngle { get ; private set ; } = 0;

    public void Fire()
    {
        if (Ammo > 0) {Ammo--;}
    }

    public int FirePower => 100;

    public void MoveForward() => Position = Position + Speed;

    public void Rotate(int angle) => CurAngle = (CurAngle + angle) % 360;

    public int Speed => 50;
}


public class Fighter() : ISpaceship
{
    public int Position { get ; private set ; } = 0;
    public int Ammo { get ; private set ; } = 50;
    public int CurAngle { get ; private set ; } = 0;

    public void Fire()
    {
        if (Ammo > 0) {Ammo--;}
    }

    public int FirePower => 20;

    public void MoveForward() => Position = Position + Speed;

    public void Rotate(int angle) => CurAngle = (CurAngle + angle) % 360;

    public int Speed => 100;
}
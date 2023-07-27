

public class WALLPOSXYZ{
    private float x;
    private float y;
    private float z;
    private float health;

    public WALLPOSXYZ(float X, float Y, float Z, float Health){
        x = X;
        y = Y;
        z = Z;
        health = Health;
    }

    public float getHealth(){
        return health;
    }

    public float getX(){
        return x;
    }

    public float getY(){
        return y;
    }

    public float getZ(){
        return z;
    }
}

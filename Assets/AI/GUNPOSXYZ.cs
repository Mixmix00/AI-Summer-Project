

public class GUNPOSXYZ{

    private float x;
    private float y;
    private float z;

    public enum GUNPOSXYZType{
        AR,
        M4,
        M16,
        AK47,
        SHOTGUN_12_GAUGE,
        SHOTGUN_8_GAUGE,
        HEAVY_SNIPER,
        LIGHT_SNIPER,
        PISTOL,
    }

    private GUNPOSXYZType type;
    

    public GUNPOSXYZ(float X, float Y, float Z, GUNPOSXYZType Type){
        x = X;
        y = Y;
        z = Z;
        type = Type;
    }

    
    public GUNPOSXYZType getType(){
        return type;
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

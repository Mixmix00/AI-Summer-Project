
public class HEALPOSXYZ{
    private float x;
    private float y;
    private float z;

    public enum HEALPOSXYZType{
        HYDROXYNORKETAMINE,
        PHENAZOPYRIDINE,
        CYCLOBENZAPRINE
    }

    private HEALPOSXYZType type;

    public HEALPOSXYZ(float X, float Y, float Z, HEALPOSXYZType Type){
        x = X;
        y = Y;
        z = Z;
        type = Type;
    }

    public HEALPOSXYZType getType(){
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

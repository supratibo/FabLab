/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package raspberryapp;

import static java.lang.Math.*;

/**
 *
 * @author thibaultfarnier
 */
public class Position {
    private double x;
    private double y;
    private double z;

    public Position(double x, double y, double z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public double getX() {
        return this.x;
    }
    public double getY() {
        return this.y;
    }
    public double getZ() {
        return this.z;
    }
    
    // Version avec 3 balises
    // dA distance à l'origine
    // dB distance au point sur l'axe des x
    // dC distance au point sur l'axe des y
    public static Position positionFromDistance(double dA, double dB, double dC) {
        double dist = 10;
        Position A = new Position(dist,0,0);
        Position B = new Position(0,dist,0);

        double x = (A.x*A.x + dA*dA - dB*dB)/(2*A.x);
        double y = (B.y*B.y + dA*dA - dC*dC)/(2*A.x);
        
        double partZ = -B.getY()*B.getY()*pow(A.getX(),4) + 2*B.getY()*B.getY()*dB*dB*A.getX()*A.getX() - B.getY()*B.getY()*pow(dA,4) + 2*B.getY()*B.getY()*dA*dA*dB*dB - B.getY()*B.getY()*pow(dB,4) - A.getX()*A.getX()*pow(B.getY(),4) + 2*A.getX()*A.getX()*dC*dC*B.getY()*B.getY() - A.getX()*A.getX()*pow(dA,4) + 2*A.getX()*A.getX()*dA*dA*dC*dC - A.getX()*A.getX()*pow(dC,4);
        // en cas de valeur proche de zero négative
        if (partZ < 0)
            partZ = -partZ;
        double z = sqrt(partZ) / (2*B.y*A.x);

        return new Position (x,y,z);
    }

    // Version avec 4 balises
    public static Position positionFromDistance(double dA, double dB, double dC, double dD) {
        double dist = 10;
        Position A = new Position(0,0,0);
        Position B = new Position(dist,0,0);
        Position C = new Position(0,dist,0);
        Position D = new Position(0,0,dist);


        double bAB = (dA*dA - dB*dB + dist*dist)/2;
        double bAC = (dA*dA - dC*dC + dist*dist)/2;
        double bAD = (dA*dA - dD*dD + dist*dist)/2;


        Position pos = new Position(bAB/B.getX(), bAC/C.getY(), bAD/D.getZ());

        return pos;
    }

    @Override
    public String toString() {
        return this.x + ", " + this.y + ", " + this.z;
    }
}

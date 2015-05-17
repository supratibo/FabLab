/*
* To change this license header, choose License Headers in Project Properties.
* To change this template file, choose Tools | Templates
* and open the template in the editor.
*/
package raspberryapp;

import java.io.*;
import java.net.*;
import java.util.*;

/**
 *
 * @author thibaultfarnier
 */
public class RaspberryApp {
    
    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        System.err.println(args.length);
        
        /*Timer timer = new Timer();
        timer.schedule(new TimerTask() {
            @Override
            public void run() {
                
                
                Position pos1;
        
        pos1 = Position.positionFromDistance(Double.parseDouble(args[0]),Double.parseDouble(args[1]),Double.parseDouble(args[2]));
        
        System.out.println(pos1);
        }
        }, 100); // delay is in ms*/
        
        Position pos1 = new Position(0, 0, 0);
        
        if (args.length == 3) {
            
            pos1 = Position.positionFromDistance(Double.parseDouble(args[0]),Double.parseDouble(args[1]),Double.parseDouble(args[2]));
            
            System.out.println(pos1);
            
        } else if (args.length == 4) {
            
            pos1 = Position.positionFromDistance(Double.parseDouble(args[0]),Double.parseDouble(args[1]),Double.parseDouble(args[2]),Double.parseDouble(args[3]));
            
            System.out.println(pos1);
        }
        
        URL url = null;
        try {
            url = new URL("http://localhost:8080/rest/items/MobileSensor/state");
        } catch (MalformedURLException exception) {
            exception.printStackTrace();
        }
        HttpURLConnection httpURLConnection = null;
        DataOutputStream dataOutputStream = null;
        try {
            httpURLConnection = (HttpURLConnection) url.openConnection();
            //httpURLConnection.setRequestProperty("Content-Type", "text/plain");
            httpURLConnection.setRequestMethod("PUT");
            //httpURLConnection.setDoInput(true);
            httpURLConnection.setDoOutput(true);
            dataOutputStream = new DataOutputStream(httpURLConnection.getOutputStream());
            //byte[] b = pos1.toString().getBytes();
            dataOutputStream.writeBytes(pos1.toString());
        } catch (IOException exception) {
            exception.printStackTrace();
        }  finally {
            if (dataOutputStream != null) {
                try {
                    dataOutputStream.flush();
                    dataOutputStream.close();
                } catch (IOException exception) {
                    exception.printStackTrace();
                }
            }
            if (httpURLConnection != null) {
                httpURLConnection.disconnect();
            }
        }
        try {
            InputStream is = httpURLConnection.getInputStream();
            BufferedReader rd = new BufferedReader(new InputStreamReader(is));
            StringBuilder response = new StringBuilder(); // or StringBuffer if not Java 5+
            String line;
            while((line = rd.readLine()) != null) {
                response.append(line);
                response.append('\r');
            }
            rd.close();
            System.out.println(response.toString());
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            if(httpURLConnection != null) {
                httpURLConnection.disconnect();
            }
        }
        
    }
}

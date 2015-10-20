/*import java.applet.Applet;
import java.awt.Color;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Image;
import java.awt.event.MouseMotionAdapter;
import java.awt.event.MouseEvent;

public class TestFold extends Applet {

  public void init() {
    buf_img_ = createImage(640,480);
    buf_g_ = buf_img_.getGraphics();
    bkgd_0_ = getImage(getDocumentBase(), "hello_world_1.png");
    bkgd_1_ = getImage(getDocumentBase(), "hello_world_0.png");

    addMouseMotionListener(new MouseMotionAdapter() {
      public void mouseMoved(MouseEvent ev) {
        mouse_x_ = ev.getX();
        mouse_y_ = ev.getY();
      }
    });

    new Thread() {
      public void run() {
        for(;;) {
          try {
            Thread.sleep(10);
          } catch(InterruptedException e) {
            Thread.currentThread().interrupt();
            break;
          }
          repaint();
        }
      }
    }.start();
  }

  public void paint(Graphics g) {
    g.drawImage(bkgd_0_,0,0,this);
    int mx = mouse_x_ + 1;
    int my = mouse_y_ + 1;
    int fx = my*my/2/mx + mx/2;
    int fy = mx*mx/2/my + my/2;
    Graphics2D g2d = (Graphics2D) g.create();
    g2d.translate(mx,my);
    g2d.rotate(Math.atan2((double) my, (double)mx-fx));
    g2d.scale(-1,1);
    g2d.drawImage(bkgd_1_,0,0,this);
    int[] xs = {0,fx,0};
    int[] ys = {fy,0,0};
    g.setColor(Color.WHITE);
    g.fillPolygon(xs,ys,3);
  }

  public void update(Graphics g) {
    buf_g_.setColor(Color.WHITE);
    buf_g_.fillRect(0,0,640,480);
    paint(buf_g_);
    g.drawImage(buf_img_,0,0,this);
  }

  Graphics buf_g_;
  Image buf_img_;
  Image bkgd_0_;
  Image bkgd_1_;
  int mouse_x_;
  int mouse_y_;
}*/
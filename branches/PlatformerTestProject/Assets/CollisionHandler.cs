/**********************************************************************************************************************
    Creator:        Tom Dubiner, Tearable World
    
    Description:
        Collision system.
    
***********************************************************************************************************************/
#region Imports
using UnityEngine;
using System.Collections;
#endregion


public class CollisionHandler : MonoBehaviour{
    #region Variables
    // Variable declarations.
    public static Random            rand = new Random();
    
    // Game screen width and height. Exactly what it looks like.
    public static int               width, height;
    
	
    public enum Shape{
        Circle,
        Rectangle
    }
	
    #endregion
    /**********************************************************************************************************************/
    public class Hitbox{
        #region Variables
        // Half the size of this hitbox (rotation independent) kept to avoid constant division and the velocity of the
        //        center of this entity.
        public Vector2              vel, halfSize;
        // The area occupied by this hitbox (if the X and Y values represent this entity's center)
        public Rectangle            hitbox;

        // The angle this entity and the Sin and Cos of that angle.
        public float                angle, sin, cos;

        // Identifier declaring what shape this hitbox defines.

        public Shape shape;
        #endregion
        /******************************************************************************************************************/
        #region Constructors/Destructors
        // Basic default constructor.
        public Hitbox(Shape shape_, Rectangle hitbox_, Vector2 vel_){
            init(shape_, hitbox_, vel_);
        }
        public void init(Shape shape_, Rectangle hitbox_, Vector2 vel_){
            this.shape = shape_;
            
            this.hitbox = hitbox_;
            this.vel = vel_;

            this.angle = 0;
            this.sin = 0; this.cos = 1;
            updateHalfSize();
        }
        #endregion
        /******************************************************************************************************************/
        #region Movement
        #region Movement_Location
        // Moves this hitbox to the given location.
        public void move(float x, float y){
            hitbox.x = x; hitbox.y = y;
        }
        public void move(Vector2 loc){
            hitbox.x = loc.x; hitbox.y = loc.y;
        }
        // Shifts this hitbox's location by the given amount.
        public void shift(float x, float y){
            hitbox.x += x; hitbox.y += y;
        }
        public void shift(Vector2 loc){
            hitbox.x += loc.x; hitbox.y += loc.y;
        }
        #endregion
        #region Movement_Size
        // Changes the width and height of this hitbox to the ones given.
        public void resize(float width, float height){
            hitbox.width = width;
            hitbox.height = height;
            
            updateHalfSize();
        }
        public void resize(Vector2 size){
            hitbox.width = size.x;
            hitbox.height = size.y;
            
            updateHalfSize();
        }
        // Expands the width and height of this hitbox by the given amount.
        public void expand(float width, float height){
            hitbox.width += width;
            hitbox.height += height;
            
            updateHalfSize();
        }
        public void expand(Vector2 size){
            hitbox.width += size.x;
            hitbox.height += size.y;
            
            updateHalfSize();
        }
        #endregion
        #region Movement_Rotation
        // Rotates this hitbox by the given angle.
        public void rotate(float angleRotated){
            angle += angleRotated;
            if(angle >= 2*Mathf.PI){ angle -= 2*Mathf.PI; }
            else if(angle <= -2*Mathf.PI){ angle += 2*Mathf.PI; }

            sin = Mathf.Sin(angle);
            cos = Mathf.Cos(angle);

            //updateHalfSize();
        }
        // Adjustes this hitbox to be facing an angle corresponding with its velocity.
        public void redirect(){
            if(vel.x == 0){
                if(vel.y == 0){ rotate(-angle); }
                else if(vel.y > 0){ rotate((float)(0.5*Mathf.PI) - angle); }
                else{ rotate((float)(1.5*Mathf.PI) - angle); }
                return;
            }
            float arcTan = Mathf.Atan(Mathf.Abs(vel.y/vel.x));
            if(vel.y > 0){
                if(vel.x > 0){
                    rotate(arcTan - angle);
                }
                else{ rotate(Mathf.PI - arcTan - angle); }
            }
            else{
                if(vel.x > 0){
                    rotate(-arcTan - angle);
                }
                else{ rotate(Mathf.PI + arcTan - angle); }
            }
        }
        #endregion
        public void updateHalfSize(){
            halfSize.x = (int)(0.5*hitbox.width);
            halfSize.y = (int)(0.5*hitbox.height);
            //halfSize.x = (int)(0.5*((cos*rect.width) - (sin*rect.height)));
            //halfSize.y = (int)(0.5*((sin*rect.width) + (cos*rect.height)));
        }
        #endregion
        /******************************************************************************************************************/
        #region Collision
        // Main control method for collisions between hitboxs, this identifies which specific collision detection method
        //      should be used and applies it, returning the result.
        public static bool collisionCheck(Hitbox a, Hitbox b){
            switch(a.shape){
                    case Shape.Circle:
                switch(b.shape){
                        case Shape.Circle:
                    // Circle - Circle collision.
                    return collisionCheckCircles(a, b);
                        case Shape.Rectangle:
                    // Circle - Rectangle collision.
                    return collisionCheckRectAndCircle(b, a);
                        default: break;
                }
                break;
                    case Shape.Rectangle:
                switch(b.shape){
                        case Shape.Circle:
                    // Rectangle - Circle collision.
                    return collisionCheckRectAndCircle(a, b);
                        case Shape.Rectangle:
                    // Rectangle - Rectangle collision.
                    return collisionCheckRects(a, b);
                        default: break;
                }
                break;
                    default: break;
            }
            return (a.hitbox.x == b.hitbox.x) && (a.hitbox.y == b.hitbox.y);
        }
        // Checks whether the given circles intersect.
        public static bool collisionCheckCircles(Hitbox a, Hitbox b){
            float sumRadius, xDiff, yDiff;

            sumRadius = a.halfSize.x + b.halfSize.x;
            xDiff = a.hitbox.x - b.hitbox.x;
            yDiff = a.hitbox.y - b.hitbox.y;

            return ((xDiff*xDiff) + (yDiff*yDiff) <= (sumRadius*sumRadius));
        }
        // Checks whether the given rectangle intersects with the given circle.
        public static bool collisionCheckRectAndCircle(Hitbox a, Hitbox b){
            float maxRadSum = a.halfSize.x + a.halfSize.y + Mathf.Max(b.halfSize.x, b.halfSize.y);
            if((Mathf.Abs(a.hitbox.x - b.hitbox.x) > maxRadSum) || (Mathf.Abs(a.hitbox.y - b.hitbox.y) > maxRadSum)){
                return false;
            }

            Vector2 alignedCenter = alignPoint(new Vector2(b.hitbox.x, b.hitbox.y), a);
            alignedCenter.x = Mathf.Abs(alignedCenter.x);
            alignedCenter.y = Mathf.Abs(alignedCenter.y);

            if(alignedCenter.x <= a.halfSize.x){
                if(alignedCenter.y <= a.halfSize.y){
                    return true;
                }
                return (alignedCenter.y < (a.halfSize.y + b.halfSize.y));
            }
            if(alignedCenter.y <= a.halfSize.y){
                return (alignedCenter.x < (a.halfSize.x + b.halfSize.x));
            }
            float distanceX, distanceY;
            distanceX = alignedCenter.x - a.halfSize.x;
            distanceY = alignedCenter.y - a.halfSize.y;
            return ((distanceX*distanceX) + (distanceY*distanceY)) < (b.halfSize.x*b.halfSize.y);
        }
        // Checks whether the given rectangles intersect. Will not detect cases where none of either rectangle's
        //      corners are touching or inside the other rectangle.
        public static bool collisionCheckRects(Hitbox a, Hitbox b){
            float maxRadSum = a.halfSize.x + a.halfSize.y + b.halfSize.x + b.halfSize.y;
            if((Mathf.Abs(a.hitbox.x - b.hitbox.x) > maxRadSum) || (Mathf.Abs(a.hitbox.y - b.hitbox.y) > maxRadSum)){
                return false;
            }
            return collisionCheckRectsDirected(a, b) || collisionCheckRectsDirected(b, a);
        }
        // Internal method for checking rectangle to rectangle collisions.
        private static bool collisionCheckRectsDirected(Hitbox a, Hitbox b){
            Vector2 center = new Vector2(b.hitbox.x, b.hitbox.y);
            b.rotate(-2*b.angle);
            Vector2 rotated_b1 = alignPoint(new Vector2(b.hitbox.x + b.halfSize.x, b.hitbox.y + b.halfSize.y), b);
            Vector2 rotated_b2 = alignPoint(new Vector2(b.hitbox.x - b.halfSize.x, b.hitbox.y + b.halfSize.y), b);
            b.rotate(-2*b.angle);
            Vector2 rotated_b3 = center - rotated_b1;
            rotated_b1 += center;
            Vector2 rotated_b4 = center - rotated_b2;
            rotated_b2 += center;
            return pointInRect(a, rotated_b1) || pointInRect(a, rotated_b2) || pointInRect(a, rotated_b3) ||
                    pointInRect(a, rotated_b4);
        }
        // Checks whether the given circle intersects with the given line segment.
        public static bool collisionCheckCircleAndLine(Hitbox circle, Vector2 a, Vector2 b){
            Vector2 segment = b - a,
                    relCircle = new Vector2(circle.hitbox.x - a.x, circle.hitbox.y - a.y),
                    closest;

            float segmentLength = segment.magnitude;
            segment.Normalize();
            float relCircleProj = Vector2.Dot(relCircle, segment);

            if(relCircleProj < 0){ closest = a; }
            else if(relCircleProj > segmentLength){ closest = b; }
            else{ closest = relCircleProj*segment; }

            Vector2 distToCircle = new Vector2(circle.hitbox.x - closest.x, circle.hitbox.y - closest.y);
            if(distToCircle.sqrMagnitude < circle.hitbox.width*circle.hitbox.height){
                return true;
            }
            return false;
        }
        // Checks whether the given point is inside the given rectangle.
        public static bool pointInRect(Hitbox a, Vector2 b){
            Vector2 alignedPoint = alignPoint(b, a);
            return (alignedPoint.x <= a.halfSize.x) && (alignedPoint.y <= a.halfSize.y) &&
                    (-alignedPoint.x <= a.halfSize.x) && (-alignedPoint.y <= a.halfSize.y);
        }
        // Aligns the given point with the given hitbox and returns it.
        public static Vector2 alignPoint(Vector2 point, Hitbox entity){
            // newPoint = (cos*(X-U) + sin*(Y-V), -sin*(X-U) + cos*(Y-V))
            Vector2 newPoint = new Vector2(point.x - entity.hitbox.x, point.y - entity.hitbox.y);
            float x = (entity.cos*newPoint.x) + (entity.sin*newPoint.y);
            float y = -(entity.sin*newPoint.x) + (entity.cos*newPoint.y);
            newPoint.x = x; newPoint.y = y;

            return newPoint;
        }
        #endregion
    }

    
    /**********************************************************************************************************************/
    public class Rectangle{
        #region Variables
        public float x, y, width, height;
        public static Rectangle Blank{
            get{ return new Rectangle(0, 0, 0, 0); }
        }
        #endregion
        /******************************************************************************************************************/
        #region Constructors/Destructors
        public Rectangle(){
            this.x = 0.0F; this.y = 0.0F;
            this.width = 0.0F;
            this.height = 0.0F;
        }
        public Rectangle(float x_, float y_){
            this.x = x_; this.y = y_;
            this.width = 0.0F;
            this.height = 0.0F;
        }
        public Rectangle(float x_, float y_, float width_, float height_){
            this.x = x_; this.y = y_;
            this.width = width_;
            this.height = height_;
        }
        #endregion
        /******************************************************************************************************************/
        #region Operators
        public static bool operator ==(Rectangle a, Rectangle b){
            return (a.x == b.x) && (a.y == b.y) && (a.width == b.width) && (a.height == b.height);
        }
        public static bool operator !=(Rectangle a, Rectangle b) {
            return (a.x != b.x) || (a.y != b.y) || (a.width != b.width) || (a.height != b.height);
        }
        public override bool Equals(object obj){ return base.Equals(obj); }
        public override int GetHashCode(){ return 0; }
        #endregion
        /******************************************************************************************************************/
        #region Other
        public void blank(){
            this.x = 0.0F; this.y = 0.0F;
            this.width = 0.0F;
            this.height = 0.0F;
        }
        public static Rectangle clone(Rectangle rect){
            return new Rectangle(rect.x, rect.y, rect.width, rect.height);
        }
        #endregion
    }
}

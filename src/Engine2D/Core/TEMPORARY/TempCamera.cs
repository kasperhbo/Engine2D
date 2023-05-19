using OpenTK.Mathematics;

namespace Engine2D.Core;

public class TempCamera
{
    private Matrix4 projectionMatrix, viewMatrix;
    public Vector2 position;
	
    public TempCamera (Vector2 position) {
        this.position = position;
        this.projectionMatrix = new Matrix4();
        this.viewMatrix = new Matrix4();
        adjustProjection();
    }
	
    public TempCamera () {
        this.position = new Vector2();
        this.projectionMatrix = new Matrix4();
        this.viewMatrix = new Matrix4();
        adjustProjection();
    }
	
    public void adjustProjection () {
        projectionMatrix = Matrix4.Identity;
        // Top Left origin
        projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, TempEngine.Get.Size.X, TempEngine.Get.Size.Y, 0, 0, 100);
    }
	
    public Matrix4 getViewMatrix () {
        Vector3 cameraFront = new Vector3(0, 0, -1);
        Vector3 cameraUp = new Vector3(0, 1, 0);
        this.viewMatrix = Matrix4.Identity;
        viewMatrix = Matrix4.LookAt(new Vector3(position.X, position.Y, 20), cameraFront + new Vector3(position.X, position.Y, 0), cameraUp);
		
        return this.viewMatrix;
    }

    public Matrix4 getProjectionMatrix () {
        return this.projectionMatrix;		
    }
}
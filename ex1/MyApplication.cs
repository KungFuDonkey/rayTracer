using System;
namespace Template
{
	class MyApplication
	{
		// member variables
		public Surface screen;
        int centerY = 200;
        int centerX = 320;
        float[][] points = new float[4][];
        // initialize
        public void Init()
		{
            points[0] = new float[2] { -1f, -0.5f};
            points[1] = new float[2] {  1f, -0.5f};
            points[2] = new float[2] {  1f,  0.5f};
            points[3] = new float[2] { -1f,  0.5f};
		}
        // tick: renders one frame
		public void Tick()
		{
            screen.Clear(0);
            rotate(points[0], 0.01f);
            rotate(points[1], 0.01f);
            rotate(points[2], 0.01f);
            rotate(points[3], 0.01f);
            screen.Line(TX(points[0][0],centerX), TY(points[0][1], centerY), TX(points[1][0], centerX), TY(points[1][1],centerY), 0xffffff); 
            screen.Line(TX(points[1][0], centerX), TY(points[1][1], centerY), TX(points[2][0], centerX), TY(points[2][1], centerY), 0xffffff);
            screen.Line(TX(points[2][0], centerX), TY(points[2][1], centerY), TX(points[3][0], centerX), TY(points[3][1], centerY), 0xffffff);
            screen.Line(TX(points[3][0], centerX), TY(points[3][1], centerY), TX(points[0][0], centerX), TY(points[0][1], centerY), 0xffffff);
        }
        public float[] rotate(float[] point, float a)
        {
            float rx = (float)(point[0] * Math.Cos(a) - point[1] * Math.Sin(a));
            float ry = (float)(point[0] * Math.Sin(a) + point[1] * Math.Cos(a));
            point[0] = rx;
            point[1] = ry;
            return point;
        }
        public int TX(float x)
        {
            x += 2f;
            x *= screen.width / 4;
            return (int)x;
        }
        public int TX(float x, int centerX)
        {
            x += 2f;
            x *= screen.width / 4;
            x += centerX - screen.width/2;
            return (int)x;
        }
        public int TY(float y)
        {
            y *= -1;
            y += 2f;
            y *= screen.height / 4;
            return (int)y;
        }
        public int TY(float y, int centery)
        {
            y = (y / screen.width) * screen.height;
            y *= -1;
            y += 2f;
            y *= screen.height / 4;
            y += centery - screen.height/2;
            return (int)y;
        }
	}
}
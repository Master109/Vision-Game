using UnityEngine;

public class BezierCurve2D
{
	public Vector2[] points = new Vector2[0];

	// public Vector2 GetPointWithNormalizedDistance (float normalizedDistance)
	// {
	// 	Vector2 output = new Vector2();
	// 	for (int i = 0; i < points.Length; i ++)
	// 	{
	// 		Vector2 point = points[i];
			
	// 	}
	// }

	// void bezierCurve (int x[] , int y[])
	// {
	// 	double xu = 0.0 , yu = 0.0 , u = 0.0;
	// 	int i = 0;
	// 	for(u = 0.0 ; u <= 1.0 ; u += 0.0001)
	// 	{
	// 		xu = pow(1-u,3)*x[0]+3*u*pow(1-u,2)*x[1]+3*pow(u,2)*(1-u)*x[2]
	// 			+pow(u,3)*x[3];
	// 		yu = pow(1-u,3)*y[0]+3*u*pow(1-u,2)*y[1]+3*pow(u,2)*(1-u)*y[2]
	// 			+pow(u,3)*y[3];
	// 		SDL_RenderDrawPoint(renderer , (int)xu , (int)yu);
	// 	}
	// }

	// void bezierCurve (Vector2[] points)
	// {
	// 	double xu = 0.0 , yu = 0.0 , u = 0.0;
	// 	int i = 0;
	// 	for(u = 0.0 ; u <= 1.0 ; u += 0.0001)
	// 	{
	// 		xu = pow(1-u,3)*x[0]+3*u*pow(1-u,2)*x[1]+3*pow(u,2)*(1-u)*x[2]
	// 			+pow(u,3)*x[3];
	// 		yu = pow(1-u,3)*y[0]+3*u*pow(1-u,2)*y[1]+3*pow(u,2)*(1-u)*y[2]
	// 			+pow(u,3)*y[3];
	// 		SDL_RenderDrawPoint(renderer , (int)xu , (int)yu);
	// 	}
	// }
}
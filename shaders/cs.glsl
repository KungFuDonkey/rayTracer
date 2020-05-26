#version 430
//tells shader how many pixels it has to edit
layout(local_size_x = 1, local_size_y = 1) in;

//image with dimensions
layout(rgba32f, binding = 0) uniform image2D img_output;
ivec2 dims;

//object arrays
uniform float spheres[];
uniform float areaLightsources[];
uniform int count[];
uniform vec3 colors[];
uniform float planes[];
uniform vec3 vertices[];
uniform float triangles[];


//functions for getting the objects out of the arrays
void getSphere(int index, out vec3 position, out float radius, out int color, out float absorption);
void getSphere(int index, out vec3 position, out float radius);
void getAreaLightsources(int index, out vec3 position, out float radius, out int color, out float emittance);
void getColor(int index, out vec3 color);
void getPlane(int index, out vec3 normal, out float d, out int color, out float absorption);
void getPlane(int index, out vec3 normal, out float d);
void getVertice(int index, out vec3 vertice);
void getTriangle(int index, out float corner1, out float corner2, out float corner3, out int color, out float absorption);
void getTriangle(int index, out float corner1, out float corner2, out float corner3);



//main
void main(){

	//initialize the color for this pixel and get invocationID for the ray
	vec3 color = vec3(0.0, 0.0, 0.0);
	ivec2 pixel_coords = ivec2(gl_GlobalInvocationID.xy);

	//initialization for the ray
	dims = imageSize(img_output);
	vec3 ray_origin = vec3((float(pixel_coords.x * 2 - dims.x) / dims.x), (float(pixel_coords.y * 2 - dims.y) / dims.x), 0);
	vec3 ray_direction = ray_origin - vec3(0, 0, -1);
	ray_direction = normalize(ray_direction);
	float t = 100000;
	float s = 0;
	vec3 next_color = vec3(0, 0, 0);
	vec3 normal = vec3(0, 0, 0);
	float absorption = 0;
	vec3 energy = vec3(1, 1, 1);
	
	//initialization for an object
	vec3 object_position;
	float d;
	float object_absorption;
	vec3 corner1;
	vec3 corner2;
	vec3 corner3;
	float c1;
	float c2;
	float c3;
	int col;
	bool collision;
	for(int i = 0; i < 5; ++i){
		//Sphere intersections, gets the next sphere in the array and checks if it collides with the ray, if it does change t, the next_color, the normal and the absorption of the ray
		float b;
		float discriminant;
		for(int k = 0; k < 10 && k < count[0]; ++k) {
			getSphere(k, object_position, d, col, object_absorption);
			b = 2.0 * dot(ray_origin - object_position, ray_direction);
			discriminant = b * b - 4 * (dot(ray_origin - object_position, ray_origin - object_position) - d * d);
			if(discriminant >= 0) {
				s = (-b - sqrt(discriminant)) / 2;
				s = s < 0 ? (-b + sqrt(discriminant)) / 2 : s;
				if(s > 0 && s < t) {
					t = s;
					getColor(col,next_color);
					normal = normalize(ray_origin + s * ray_direction - object_position);
					absorption = object_absorption;
				}
			}

		}
		//Plane intersections, gets the next plane in the array and checks if it collides with the ray, if it does change t, the next_color, the normal and the absorption of the ray
		for(int k = 0; k < 10 && k < count[1]; ++k) {
			getPlane(k, object_position, d, col, object_absorption);
			if(dot(ray_direction, object_position) > 0) {
				s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
				if(s > 0 && s < t) {
					t = s;
					getColor(col,next_color);
					normal = -object_position;
					absorption = object_absorption;
				}
			}

		}

//		Triangle intersections, gets the next triangle in the array and checks if it collides with the ray, if it does change t, the next_color, the normal and the absorption of the ray
		for(int k = 0; k < 20 && k < count[2]; ++k){
			getTriangle(k, c1, c2, c3, col, object_absorption);
			getVertice(int(c1),corner1);
			getVertice(int(c2),corner2);
			getVertice(int(c3),corner3);
			object_position = normalize(cross(corner2 - corner1, corner3 - corner1));
			d = -dot(object_position, corner1);
			if(dot(ray_direction, object_position) > 0) {
				s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
				if(s > 0 && s < t) {
					if(dot(object_position,cross(corner2 - corner1, ray_origin + s * ray_direction - corner1)) >= 0){
						if(dot(object_position,cross(corner3 - corner2, ray_origin + s * ray_direction - corner2)) >= 0){
							if(dot(object_position,cross(corner1 - corner3, ray_origin + s * ray_direction - corner3)) >= 0){
								t = s;
								getColor(col,next_color);
								normal = -object_position;
								absorption = object_absorption;						
							}						
						}						
					}
				}
			}
		}
		//check if the ray has collided with something
		if(t != 100000){

			//update the ray energy, if energy == 0 then stop the loop as there will be no colors left to write 
			energy *= next_color;
			if(energy == vec3(0,0,0)) break;

			//create point of intersection 
			ray_origin = ray_origin + t * ray_direction;

			//lightsources initialization
			vec3 lightsource_position;
			float lightsource_radius;
			float lightsource_emittance;

			//arealightsources, gets arealightsource and calculates the luminance
			for(int j = 0; j < 10 && j < count[4]; ++j){
				s = 0;
				getAreaLightsources(j, lightsource_position, lightsource_radius, col, lightsource_emittance);

				//get direction of the lightsource and check if it has an -90 < angle < 90 otherwise disregard this arealight
				vec3 light_direction = lightsource_position - ray_origin;


				//pre calculations for the lightsource
				lightsource_emittance /= (12.456 * length(light_direction) * length(light_direction));
				float tmax = length(light_direction) - 0.0002;
				light_direction = normalize(light_direction);
				float angle = dot(normal, light_direction);
				if(angle < 0){
					continue;
				}
				ray_origin += 0.0001 * light_direction;

				collision = false;
				//sphere intersection
				for(int k = 0; k < 10 && k < count[0]; ++k) {
					getSphere(k, object_position, d);
					b = 2.0 * dot(ray_origin - object_position, light_direction);
					discriminant = b * b - 4 * (dot(ray_origin - object_position, ray_origin - object_position) - d * d);
					if(discriminant >= 0) {
						s = (-b - sqrt(discriminant)) / 2;
						if(t < 0) {
							s = (-b + sqrt(discriminant)) / 2;
						}
						collision = s > 0 && s < tmax;
						if(collision)
							break;
					}

				}
				//plane intersection
				for(int k = 0; k < 10 && k < count[1]; ++k) {
					if(collision) 
						break;
					getPlane(k, object_position, d);
					if(dot(light_direction, object_position) > 0.0001) {
						s = -(dot(ray_origin, object_position) + d) / dot(light_direction, object_position);
						collision = s > 0 && s < tmax;
					}

				}
				//triangle intersection
				for(int k = 0; k < 20 && k < count[2]; ++k){
					if(collision)
						break;
					getTriangle(k, c1, c2, c3);
					getVertice(int(c1),corner1);
					getVertice(int(c2),corner2);
					getVertice(int(c3),corner3);
					object_position = normalize(cross(corner2 - corner1, corner3 - corner1));
					d = -dot(object_position, corner1);
					if(dot(light_direction, object_position) > 0) {
						s = -(dot(ray_origin, object_position) + d) / dot(light_direction, object_position);
						if(s > 0 && s < tmax) {
							if(dot(object_position,cross(corner2 - corner1, ray_origin + s * light_direction - corner1)) >= 0){
								if(dot(object_position,cross(corner3 - corner2, ray_origin + s * light_direction - corner2)) >= 0){
									if(dot(object_position,cross(corner1 - corner3, ray_origin + s * light_direction - corner3)) >= 0){
										collision = true;				
									}						
								}						
							}
						}
					}
				}
				//if there is no collision change the color
				if(!collision){
					getColor(col,next_color);
					color += next_color * lightsource_emittance * energy * angle * absorption;
				}
			}

			//if absorption == 1 then the ray will be absorped otherwise reflect the ray
			if(absorption != 1){
				ray_direction = ray_direction - 2 * dot(normal, ray_direction) * normal; //use reflect?
				ray_origin += ray_direction * 0.0001;
				t = 100000;
			}
			else
				break;
		}
		else
			break;
	}
	//make sure the colors are within the rgb range
	color.x = (-1 / (1 + color.x)) + 1;
	color.y = (-1 / (1 + color.y)) + 1;
	color.z = (-1 / (1 + color.z)) + 1;

	//store the pixel color in the image
	imageStore(img_output, pixel_coords, vec4(color,1));
}

//void for getting the spheres out of the array, as GLSL doesn't have dynamic looping it has to be done like this
void getSphere(int index, out vec3 position, out float radius, out int color, out float absorption){
	if(index == 0){
		position = vec3(spheres[0], spheres[1], spheres[2]);
		radius = (spheres[3]);
		color = int(spheres[4]);
		absorption = (spheres[5]);
	}
	else if(index == 1){
		position = vec3(spheres[6], spheres[7], spheres[8]);
		radius = (spheres[9]);
		color = int(spheres[10]);
		absorption = (spheres[11]);
	}
	else if(index == 2){
		position = vec3(spheres[12], spheres[13], spheres[14]);
		radius = (spheres[15]);
		color = int(spheres[16]);
		absorption = (spheres[17]);
	}
	else if(index == 3){
		position = vec3(spheres[18], spheres[19], spheres[20]);
		radius = (spheres[21]);
		color = int(spheres[22]);
		absorption = (spheres[23]);
	}
	else if(index == 4){
		position = vec3(spheres[24], spheres[25], spheres[26]);
		radius = (spheres[27]);
		color = int(spheres[28]);
		absorption = (spheres[29]);
	}
	else if(index == 5){
		position = vec3(spheres[30], spheres[31], spheres[32]);
		radius = (spheres[33]);
		color = int(spheres[34]);
		absorption = (spheres[35]);
	}
	else if(index == 6){
		position = vec3(spheres[36], spheres[37], spheres[38]);
		radius = (spheres[39]);
		color = int(spheres[40]);
		absorption = (spheres[41]);
	}
	else if(index == 7){
		position = vec3(spheres[42], spheres[43], spheres[44]);
		radius = (spheres[45]);
		color = int(spheres[46]);
		absorption = (spheres[47]);
	}
	else if(index == 8){
		position = vec3(spheres[48], spheres[49], spheres[50]);
		radius = (spheres[51]);
		color = int(spheres[52]);
		absorption = (spheres[53]);
	}
}
void getSphere(int index, out vec3 position, out float radius){
	if(index == 0){
		position = vec3(spheres[0], spheres[1], spheres[2]);
		radius = (spheres[3]);
	}
	else if(index == 1){
		position = vec3(spheres[6], spheres[7], spheres[8]);
		radius = (spheres[9]);
	}
	else if(index == 2){
		position = vec3(spheres[12], spheres[13], spheres[14]);
		radius = (spheres[15]);
	}
	else if(index == 3){
		position = vec3(spheres[18], spheres[19], spheres[20]);
		radius = (spheres[21]);
	}
	else if(index == 4){
		position = vec3(spheres[24], spheres[25], spheres[26]);
		radius = (spheres[27]);
	}
	else if(index == 5){
		position = vec3(spheres[30], spheres[31], spheres[32]);
		radius = (spheres[33]);
	}
	else if(index == 6){
		position = vec3(spheres[36], spheres[37], spheres[38]);
		radius = (spheres[39]);
	}
	else if(index == 7){
		position = vec3(spheres[42], spheres[43], spheres[44]);
		radius = (spheres[45]);
	}
	else if(index == 8){
		position = vec3(spheres[48], spheres[49], spheres[50]);
		radius = (spheres[51]);
	}
	else if(index == 9){
		position = vec3(spheres[54], spheres[55], spheres[56]);
		radius = (spheres[57]);
	}
}


//void for getting the planes out of the array, as GLSL doesn't have dynamic looping it has to be done like this
void getPlane(int index, out vec3 normal, out float d, out int color, out float absorption){
	if(index == 0){
		normal = vec3(planes[0], planes[1], planes[2]);
		d = (planes[3]);
		color = int(planes[4]);
		absorption = (planes[5]);
	}
	else if(index == 1){
		normal = vec3(planes[6], planes[7], planes[8]);
		d = (planes[9]);
		color = int(planes[10]);
		absorption = (planes[11]);
	}
	else if(index == 2){
		normal = vec3(planes[12], planes[13], planes[14]);
		d = (planes[15]);
		color = int(planes[16]);
		absorption = (planes[17]);
	}
	else if(index == 3){
		normal = vec3(planes[18], planes[19], planes[20]);
		d = (planes[21]);
		color = int(planes[22]);
		absorption = (planes[23]);
	}
	else if(index == 4){
		normal = vec3(planes[24], planes[25], planes[26]);
		d = (planes[27]);
		color = int(planes[28]);
		absorption = (planes[29]);
	}
	else if(index == 5){
		normal = vec3(planes[30], planes[31], planes[32]);
		d = (planes[33]);
		color = int(planes[34]);
		absorption = (planes[35]);
	}
	else if(index == 6){
		normal = vec3(planes[36], planes[37], planes[38]);
		d = (planes[39]);
		color = int(planes[40]);
		absorption = (planes[41]);
	}
	else if(index == 7){
		normal = vec3(planes[42], planes[43], planes[44]);
		d = (planes[45]);
		color = int(planes[46]);
		absorption = (planes[47]);
	}
	else if(index == 8){
		normal = vec3(planes[48], planes[49], planes[50]);
		d = (planes[51]);
		color = int(planes[52]);
		absorption = (planes[53]);
	}
	else if(index == 9){
		normal = vec3(planes[54], planes[55], planes[56]);
		d = (planes[57]);
		color = int(planes[58]);
		absorption = (planes[59]);
	}
}
void getPlane(int index, out vec3 normal, out float d){
	if(index == 0){
		normal = vec3(planes[0], planes[1], planes[2]);
		d = (planes[3]);
	}
	else if(index == 1){
		normal = vec3(planes[6], planes[7], planes[8]);
		d = (planes[9]);
	}
	else if(index == 2){
		normal = vec3(planes[12], planes[13], planes[14]);
		d = (planes[15]);
	}
	else if(index == 3){
		normal = vec3(planes[18], planes[19], planes[20]);
		d = (planes[21]);
	}
	else if(index == 4){
		normal = vec3(planes[24], planes[25], planes[26]);
		d = (planes[27]);
	}
	else if(index == 5){
		normal = vec3(planes[30], planes[31], planes[32]);
		d = (planes[33]);
	}
	else if(index == 6){
		normal = vec3(planes[36], planes[37], planes[38]);
		d = (planes[39]);
	}
	else if(index == 7){
		normal = vec3(planes[42], planes[43], planes[44]);
		d = (planes[45]);
	}
	else if(index == 8){
		normal = vec3(planes[48], planes[49], planes[50]);
		d = (planes[51]);
	}
	else if(index == 9){
		normal = vec3(planes[54], planes[55], planes[56]);
		d = (planes[57]);
	}
}



//void for getting the arealightsources out of the array, as GLSL doesn't have dynamic looping it has to be done like this
void getAreaLightsources(int index, out vec3 position, out float radius, out int color, out float emittance){
	if(index == 0){
		position = vec3(areaLightsources[0], areaLightsources[1], areaLightsources[2]);
		radius = (areaLightsources[3]);
		color = int(areaLightsources[4]);
		emittance = (areaLightsources[5]);
	}
	else if(index == 1){
		position = vec3(areaLightsources[6], areaLightsources[7], areaLightsources[8]);
		radius = (areaLightsources[9]);
		color = int(areaLightsources[10]);
		emittance = (areaLightsources[11]);
	}
	else if(index == 2){
		position = vec3(areaLightsources[12], areaLightsources[13], areaLightsources[14]);
		radius = (areaLightsources[15]);
		color = int(areaLightsources[16]);
		emittance = (areaLightsources[17]);
	}
	else if(index == 3){
		position = vec3(areaLightsources[18], areaLightsources[19], areaLightsources[20]);
		radius = (areaLightsources[21]);
		color = int(areaLightsources[22]);
		emittance = (areaLightsources[23]);
	}
	else if(index == 4){
		position = vec3(areaLightsources[24], areaLightsources[25], areaLightsources[26]);
		radius = (areaLightsources[27]);
		color = int(areaLightsources[28]);
		emittance = (areaLightsources[29]);
	}
	else if(index == 5){
		position = vec3(areaLightsources[30], areaLightsources[31], areaLightsources[32]);
		radius = (areaLightsources[33]);
		color = int(areaLightsources[34]);
		emittance = (areaLightsources[35]);
	}
	else if(index == 6){
		position = vec3(areaLightsources[36], areaLightsources[37], areaLightsources[38]);
		radius = (areaLightsources[39]);
		color = int(areaLightsources[40]);
		emittance = (areaLightsources[41]);
	}
	else if(index == 7){
		position = vec3(areaLightsources[42], areaLightsources[43], areaLightsources[44]);
		radius = (areaLightsources[45]);
		color = int(areaLightsources[46]);
		emittance = (areaLightsources[47]);
	}
	else if(index == 8){
		position = vec3(areaLightsources[48], areaLightsources[49], areaLightsources[50]);
		radius = (areaLightsources[51]);
		color = int(areaLightsources[52]);
		emittance = (areaLightsources[53]);
	}
	else if(index == 9){
		position = vec3(areaLightsources[54], areaLightsources[55], areaLightsources[56]);
		radius = (areaLightsources[57]);
		color = int(areaLightsources[58]);
		emittance = (areaLightsources[59]);
	}
}


//void for getting the colors out of the array, as GLSL doesn't have dynamic looping it has to be done like this
void getColor(int index, out vec3 color){
	if(index == 0){
		color = colors[0];
	}
	else if(index == 1){
		color = colors[1];
	}
	else if(index == 2){
		color = colors[2];
	}
	else if(index == 3){
		color = colors[3];
	}
	else if(index == 4){
		color = colors[4];
	}
	else if(index == 5){
		color = colors[5];
	}
	else if(index == 6){
		color = colors[6];
	}
	else if(index == 7){
		color = colors[7];
	}
	else if(index == 8){
		color = colors[8];
	}
	else if(index == 9){
		color = colors[9];
	}
	else if(index == 10){
		color = colors[10];
	}
	else if(index == 11){
		color = colors[11];
	}
	else if(index == 12){
		color = colors[12];
	}
	else if(index == 13){
		color = colors[13];
	}
	else if(index == 14){
		color = colors[14];
	}
	else if(index == 15){
		color = colors[15];
	}
	else if(index == 16){
		color = colors[16];
	}
	else if(index == 17){
		color = colors[17];
	}
	else if(index == 18){
		color = colors[18];
	}
	else if(index == 19){
		color = colors[19];
	}
}


//void for getting the triangles out of the array, as GLSL doesn't have dynamic looping it has to be done like this
void getTriangle(int index, out float corner1, out float corner2, out float corner3, out int color, out float absorption){
	if(index == 0){
		corner1 = (triangles[0]);
		corner2 = (triangles[1]);
		corner3 = (triangles[2]);
		color = int(triangles[3]);
		absorption = (triangles[4]);
	}
	else if(index == 1){
		corner1 = (triangles[5]);
		corner2 = (triangles[6]);
		corner3 = (triangles[7]);
		color = int(triangles[8]);
		absorption = (triangles[9]);
	}
	else if(index == 2){
		corner1 = (triangles[10]);
		corner2 = (triangles[11]);
		corner3 = (triangles[12]);
		color = int(triangles[13]);
		absorption = (triangles[14]);
	}
	else if(index == 3){
		corner1 = (triangles[15]);
		corner2 = (triangles[16]);
		corner3 = (triangles[17]);
		color = int(triangles[18]);
		absorption = (triangles[19]);
	}
	else if(index == 4){
		corner1 = (triangles[20]);
		corner2 = (triangles[21]);
		corner3 = (triangles[22]);
		color = int(triangles[23]);
		absorption = (triangles[24]);
	}
	else if(index == 5){
		corner1 = (triangles[25]);
		corner2 = (triangles[26]);
		corner3 = (triangles[27]);
		color = int(triangles[28]);
		absorption = (triangles[29]);
	}
	else if(index == 6){
		corner1 = (triangles[30]);
		corner2 = (triangles[31]);
		corner3 = (triangles[32]);
		color = int(triangles[33]);
		absorption = (triangles[34]);
	}
	else if(index == 7){
		corner1 = (triangles[35]);
		corner2 = (triangles[36]);
		corner3 = (triangles[37]);
		color = int(triangles[38]);
		absorption = (triangles[39]);
	}
	else if(index == 8){
		corner1 = (triangles[40]);
		corner2 = (triangles[41]);
		corner3 = (triangles[42]);
		color = int(triangles[43]);
		absorption = (triangles[44]);
	}
	else if(index == 9){
		corner1 = (triangles[45]);
		corner2 = (triangles[46]);
		corner3 = (triangles[47]);
		color = int(triangles[48]);
		absorption = (triangles[49]);
	}
	else if(index == 10){
		corner1 = (triangles[50]);
		corner2 = (triangles[51]);
		corner3 = (triangles[52]);
		color = int(triangles[53]);
		absorption = (triangles[54]);
	}
	else if(index == 11){
		corner1 = (triangles[55]);
		corner2 = (triangles[56]);
		corner3 = (triangles[57]);
		color = int(triangles[58]);
		absorption = (triangles[59]);
	}
	else if(index == 12){
		corner1 = (triangles[60]);
		corner2 = (triangles[61]);
		corner3 = (triangles[62]);
		color = int(triangles[63]);
		absorption = (triangles[64]);
	}
	else if(index == 13){
		corner1 = (triangles[65]);
		corner2 = (triangles[66]);
		corner3 = (triangles[67]);
		color = int(triangles[68]);
		absorption = (triangles[69]);
	}
	else if(index == 14){
		corner1 = (triangles[70]);
		corner2 = (triangles[71]);
		corner3 = (triangles[72]);
		color = int(triangles[73]);
		absorption = (triangles[74]);
	}
	else if(index == 15){
		corner1 = (triangles[75]);
		corner2 = (triangles[76]);
		corner3 = (triangles[77]);
		color = int(triangles[78]);
		absorption = (triangles[79]);
	}
	else if(index == 16){
		corner1 = (triangles[80]);
		corner2 = (triangles[81]);
		corner3 = (triangles[82]);
		color = int(triangles[83]);
		absorption = (triangles[84]);
	}
	else if(index == 17){
		corner1 = (triangles[85]);
		corner2 = (triangles[86]);
		corner3 = (triangles[87]);
		color = int(triangles[88]);
		absorption = (triangles[89]);
	}
	else if(index == 18){
		corner1 = (triangles[90]);
		corner2 = (triangles[91]);
		corner3 = (triangles[92]);
		color = int(triangles[93]);
		absorption = (triangles[94]);
	}
	else if(index == 19){
		corner1 = (triangles[95]);
		corner2 = (triangles[96]);
		corner3 = (triangles[97]);
		color = int(triangles[98]);
		absorption = (triangles[99]);
	}
}
void getTriangle(int index, out float corner1, out float corner2, out float corner3){
	if(index == 0){
		corner1 = (triangles[0]);
		corner2 = (triangles[1]);
		corner3 = (triangles[2]);
	}
	else if(index == 1){
		corner1 = (triangles[5]);
		corner2 = (triangles[6]);
		corner3 = (triangles[7]);
	}
	else if(index == 2){
		corner1 = (triangles[10]);
		corner2 = (triangles[11]);
		corner3 = (triangles[12]);
	}
	else if(index == 3){
		corner1 = (triangles[15]);
		corner2 = (triangles[16]);
		corner3 = (triangles[17]);
	}
	else if(index == 4){
		corner1 = (triangles[20]);
		corner2 = (triangles[21]);
		corner3 = (triangles[22]);
	}
	else if(index == 5){
		corner1 = (triangles[25]);
		corner2 = (triangles[26]);
		corner3 = (triangles[27]);
	}
	else if(index == 6){
		corner1 = (triangles[30]);
		corner2 = (triangles[31]);
		corner3 = (triangles[32]);
	}
	else if(index == 7){
		corner1 = (triangles[35]);
		corner2 = (triangles[36]);
		corner3 = (triangles[37]);
	}
	else if(index == 8){
		corner1 = (triangles[40]);
		corner2 = (triangles[41]);
		corner3 = (triangles[42]);
	}
	else if(index == 9){
		corner1 = (triangles[45]);
		corner2 = (triangles[46]);
		corner3 = (triangles[47]);
	}
	else if(index == 10){
		corner1 = (triangles[50]);
		corner2 = (triangles[51]);
		corner3 = (triangles[52]);
	}
	else if(index == 11){
		corner1 = (triangles[55]);
		corner2 = (triangles[56]);
		corner3 = (triangles[57]);
	}
	else if(index == 12){
		corner1 = (triangles[60]);
		corner2 = (triangles[61]);
		corner3 = (triangles[62]);
	}
	else if(index == 13){
		corner1 = (triangles[65]);
		corner2 = (triangles[66]);
		corner3 = (triangles[67]);
	}
	else if(index == 14){
		corner1 = (triangles[70]);
		corner2 = (triangles[71]);
		corner3 = (triangles[72]);
	}
	else if(index == 15){
		corner1 = (triangles[75]);
		corner2 = (triangles[76]);
		corner3 = (triangles[77]);
	}
	else if(index == 16){
		corner1 = (triangles[80]);
		corner2 = (triangles[81]);
		corner3 = (triangles[82]);
	}
	else if(index == 17){
		corner1 = (triangles[85]);
		corner2 = (triangles[86]);
		corner3 = (triangles[87]);
	}
	else if(index == 18){
		corner1 = (triangles[90]);
		corner2 = (triangles[91]);
		corner3 = (triangles[92]);
	}
	else if(index == 19){
		corner1 = (triangles[95]);
		corner2 = (triangles[96]);
		corner3 = (triangles[97]);
	}
}

void getVertice(int index, out vec3 vertice){
	if(index == 0){
		vertice = vertices[0];
	}
	else if(index == 1){
		vertice = vertices[1];
	}
	else if(index == 2){
		vertice = vertices[2];
	}
	else if(index == 3){
		vertice = vertices[3];
	}
	else if(index == 4){
		vertice = vertices[4];
	}
	else if(index == 5){
		vertice = vertices[5];
	}
	else if(index == 6){
		vertice = vertices[6];
	}
	else if(index == 7){
		vertice = vertices[7];
	}
	else if(index == 8){
		vertice = vertices[8];
	}
	else if(index == 9){
		vertice = vertices[9];
	}
	else if(index == 10){
		vertice = vertices[10];
	}
	else if(index == 11){
		vertice = vertices[11];
	}
	else if(index == 12){
		vertice = vertices[12];
	}
	else if(index == 13){
		vertice = vertices[13];
	}
	else if(index == 14){
		vertice = vertices[14];
	}
	else if(index == 15){
		vertice = vertices[15];
	}
	else if(index == 16){
		vertice = vertices[16];
	}
	else if(index == 17){
		vertice = vertices[17];
	}
	else if(index == 18){
		vertice = vertices[18];
	}
	else if(index == 19){
		vertice = vertices[19];
	}
}






#version 430
layout(local_size_x = 1, local_size_y = 1) in;
layout(rgba32f, binding = 0) uniform image2D img_output;

float spheres[];
float areaLightsources[];
int count[];

ivec2 dims;

void sphereIntersection(vec3 ray_origin, vec3 ray_direction, vec3 sphere_position, float d, bool lightray, inout float t);
void getSphere(int index, out vec3 position, out float radius, out vec3 color, out float absorption);
void getAreaLightsources(int index, out vec3 position, out float radius, out vec3 color, out float emittance);

void main(){
	vec4 color = vec4(0.0, 0.0, 0.0, 1.0);
	ivec2 pixel_coords = ivec2(gl_GlobalInvocationID.xy);

	float max_x = 1;
	float max_y = 1;
	dims = imageSize(img_output);
	float x = (float(pixel_coords.x * 2 - dims.x) / dims.x);
	float y = (float(pixel_coords.y * 2 - dims.y) / dims.x);

	vec3 ray_origin = vec3(x, y, 0);
	vec3 ray_direction = ray_origin - vec3(0, 0, -1);
	ray_direction = normalize(ray_direction);
	float t = 1 / 0;
	vec3 next_color = vec3(0, 0, 0);
	vec3 normal = vec3(0, 0, 0);
	float absorption = 0;
	vec3 energy = vec3(1, 1, 1);

	for(int i = 0; i < 5; ++i) {
		vec3 object_position;
		float d;
		float object_absorption;
		vec3 object_color;

		//Sphere intersections
		for(int k = 0; k < 10 && k < count[0]; ++k) {
			float s = 1 / 0;
			getSphere(k, object_position, d, object_color, absorption);
			sphereIntersection(ray_origin, ray_direction, object_position, d, false, s);

			if(s < t && s > 0) {
				t = s;
				next_color = object_color;
				normal = normalize(ray_origin + s * ray_direction - object_position);
				absorption = object_absorption;
			}
		}


		if(t != 1 / 0) {
			vec3 point_of_intersection = ray_origin + t * ray_direction;
			energy *= next_color;
			
			//Area light calculations
			for(int j = 0; j < 10 && j < count[1]; ++j) {
				getAreaLightsources(j, object_position, d, object_color, object_absorption);
				vec3 light_direction  = object_position - point_of_intersection;
				float angle = dot(normal, light_direction);
				if(angle < 0) {
					continue;
				}
				float illumination = object_absorption / (12.456 * length(light_direction) * length(light_direction));
				float tmax = length(light_direction) - 2 * 0.00001;
				light_direction = normalize(light_direction);
				point_of_intersection += 0.00001 * light_direction;
				bool collision = false;

				for(int k = 0; k < 10; ++k) {
					float s = 1 / 0;
					getSphere(k, object_position, d, object_color, absorption);
					sphereIntersection(point_of_intersection, light_direction, object_position, d, true, s);

					if(s < tmax && s > 0) {
						collision = true;
						break;
					}
					if(collision) {
						break;
					}
				}

				if(!collision) {
					color += illumination * dot(normal, light_direction) * object_color * energy * absorption;
				}
			}

			if(absorption != 1) {
				ray_origin = point_of_intersection;
				ray_direction = ray_direction - 2 * dot(normal, ray_direction) * normal;
				ray_origin += ray_direction * 0.00001;
				t = 1 / 0;
			}
		}
	}

	color.x = (-1 / (1 + color.x)) + 1;
	color.y = (-1 / (1 + color.y)) + 1;
	color.z = (-1 / (1 + color.z)) + 1;
	imageStore(img_output, pixel_coords, color);
}

void objectIntersection(vec3 ray_origin, vec3 ray_direction) {
}

void sphereIntersection(vec3 ray_origin, vec3 ray_direction, vec3 sphere_position, float d, bool lightray, inout float t) {
	vec3 conversion = ray_origin - sphere_position;
	float b = 2.0 * dot(conversion, ray_direction);
	float c = dot(conversion, conversion) - d * d;
	float discriminant = b * b - 4 * c;

	if(discriminant >= 0) {
		t = (-b - sqrt(discriminant)) / 2;

		if(t < 0) {
			t = (-b + sqrt(discriminant)) / 2;
		}
	}
}

void getSphere(int index, out vec3 position, out float radius, out vec3 color, out float absorption){
	if(index == 0){
		position = vec3(spheres[0], spheres[1], spheres[2]);
		radius = float(spheres[3]);
		color = vec3(spheres[4], spheres[5], spheres[6]);
		absorption = float(spheres[7]);
	}
	else if(index == 1){
		position = vec3(spheres[8], spheres[9], spheres[10]);
		radius = float(spheres[11]);
		color = vec3(spheres[12], spheres[13], spheres[14]);
		absorption = float(spheres[15]);
	}
	else if(index == 2){
		position = vec3(spheres[16], spheres[17], spheres[18]);
		radius = float(spheres[19]);
		color = vec3(spheres[20], spheres[21], spheres[22]);
		absorption = float(spheres[23]);
	}
	else if(index == 3){
		position = vec3(spheres[24], spheres[25], spheres[26]);
		radius = float(spheres[27]);
		color = vec3(spheres[28], spheres[29], spheres[30]);
		absorption = float(spheres[31]);
	}
	else if(index == 4){
		position = vec3(spheres[32], spheres[33], spheres[34]);
		radius = float(spheres[35]);
		color = vec3(spheres[36], spheres[37], spheres[38]);
		absorption = float(spheres[39]);
	}
	else if(index == 5){
		position = vec3(spheres[40], spheres[41], spheres[42]);
		radius = float(spheres[43]);
		color = vec3(spheres[44], spheres[45], spheres[46]);
		absorption = float(spheres[47]);
	}
	else if(index == 6){
		position = vec3(spheres[48], spheres[49], spheres[50]);
		radius = float(spheres[51]);
		color = vec3(spheres[52], spheres[53], spheres[54]);
		absorption = float(spheres[55]);
	}
	else if(index == 7){
		position = vec3(spheres[56], spheres[57], spheres[58]);
		radius = float(spheres[59]);
		color = vec3(spheres[60], spheres[61], spheres[62]);
		absorption = float(spheres[63]);
	}
	else if(index == 8){
		position = vec3(spheres[64], spheres[65], spheres[66]);
		radius = float(spheres[67]);
		color = vec3(spheres[68], spheres[69], spheres[70]);
		absorption = float(spheres[71]);
	}
	else if(index == 9){
		position = vec3(spheres[72], spheres[73], spheres[74]);
		radius = float(spheres[75]);
		color = vec3(spheres[76], spheres[77], spheres[78]);
		absorption = float(spheres[79]);
	}
	else if(index == 10){
		position = vec3(spheres[80], spheres[81], spheres[82]);
		radius = float(spheres[83]);
		color = vec3(spheres[84], spheres[85], spheres[86]);
		absorption = float(spheres[87]);
	}
	else if(index == 11){
		position = vec3(spheres[88], spheres[89], spheres[90]);
		radius = float(spheres[91]);
		color = vec3(spheres[92], spheres[93], spheres[94]);
		absorption = float(spheres[95]);
	}
	else if(index == 12){
		position = vec3(spheres[96], spheres[97], spheres[98]);
		radius = float(spheres[99]);
		color = vec3(spheres[100], spheres[101], spheres[102]);
		absorption = float(spheres[103]);
	}
	else if(index == 13){
		position = vec3(spheres[104], spheres[105], spheres[106]);
		radius = float(spheres[107]);
		color = vec3(spheres[108], spheres[109], spheres[110]);
		absorption = float(spheres[111]);
	}
	else if(index == 14){
		position = vec3(spheres[112], spheres[113], spheres[114]);
		radius = float(spheres[115]);
		color = vec3(spheres[116], spheres[117], spheres[118]);
		absorption = float(spheres[119]);
	}
	else if(index == 15){
		position = vec3(spheres[120], spheres[121], spheres[122]);
		radius = float(spheres[123]);
		color = vec3(spheres[124], spheres[125], spheres[126]);
		absorption = float(spheres[127]);
	}
	else if(index == 16){
		position = vec3(spheres[128], spheres[129], spheres[130]);
		radius = float(spheres[131]);
		color = vec3(spheres[132], spheres[133], spheres[134]);
		absorption = float(spheres[135]);
	}
	else if(index == 17){
		position = vec3(spheres[136], spheres[137], spheres[138]);
		radius = float(spheres[139]);
		color = vec3(spheres[140], spheres[141], spheres[142]);
		absorption = float(spheres[143]);
	}
	else if(index == 18){
		position = vec3(spheres[144], spheres[145], spheres[146]);
		radius = float(spheres[147]);
		color = vec3(spheres[148], spheres[149], spheres[150]);
		absorption = float(spheres[151]);
	}
	else if(index == 19){
		position = vec3(spheres[152], spheres[153], spheres[154]);
		radius = float(spheres[155]);
		color = vec3(spheres[156], spheres[157], spheres[158]);
		absorption = float(spheres[159]);
	}
}

void getAreaLightsources(int index, out vec3 position, out float radius, out vec3 color, out float emittance){
	if(index == 0){
		position = vec3(areaLightsources[0], areaLightsources[1], areaLightsources[2]);
		radius = float(areaLightsources[3]);
		color = vec3(areaLightsources[4], areaLightsources[5], areaLightsources[6]);
		emittance = float(areaLightsources[7]);
	}
	else if(index == 1){
		position = vec3(areaLightsources[8], areaLightsources[9], areaLightsources[10]);
		radius = float(areaLightsources[11]);
		color = vec3(areaLightsources[12], areaLightsources[13], areaLightsources[14]);
		emittance = float(areaLightsources[15]);
	}
	else if(index == 2){
		position = vec3(areaLightsources[16], areaLightsources[17], areaLightsources[18]);
		radius = float(areaLightsources[19]);
		color = vec3(areaLightsources[20], areaLightsources[21], areaLightsources[22]);
		emittance = float(areaLightsources[23]);
	}
	else if(index == 3){
		position = vec3(areaLightsources[24], areaLightsources[25], areaLightsources[26]);
		radius = float(areaLightsources[27]);
		color = vec3(areaLightsources[28], areaLightsources[29], areaLightsources[30]);
		emittance = float(areaLightsources[31]);
	}
	else if(index == 4){
		position = vec3(areaLightsources[32], areaLightsources[33], areaLightsources[34]);
		radius = float(areaLightsources[35]);
		color = vec3(areaLightsources[36], areaLightsources[37], areaLightsources[38]);
		emittance = float(areaLightsources[39]);
	}
	else if(index == 5){
		position = vec3(areaLightsources[40], areaLightsources[41], areaLightsources[42]);
		radius = float(areaLightsources[43]);
		color = vec3(areaLightsources[44], areaLightsources[45], areaLightsources[46]);
		emittance = float(areaLightsources[47]);
	}
	else if(index == 6){
		position = vec3(areaLightsources[48], areaLightsources[49], areaLightsources[50]);
		radius = float(areaLightsources[51]);
		color = vec3(areaLightsources[52], areaLightsources[53], areaLightsources[54]);
		emittance = float(areaLightsources[55]);
	}
	else if(index == 7){
		position = vec3(areaLightsources[56], areaLightsources[57], areaLightsources[58]);
		radius = float(areaLightsources[59]);
		color = vec3(areaLightsources[60], areaLightsources[61], areaLightsources[62]);
		emittance = float(areaLightsources[63]);
	}
	else if(index == 8){
		position = vec3(areaLightsources[64], areaLightsources[65], areaLightsources[66]);
		radius = float(areaLightsources[67]);
		color = vec3(areaLightsources[68], areaLightsources[69], areaLightsources[70]);
		emittance = float(areaLightsources[71]);
	}
	else if(index == 9){
		position = vec3(areaLightsources[72], areaLightsources[73], areaLightsources[74]);
		radius = float(areaLightsources[75]);
		color = vec3(areaLightsources[76], areaLightsources[77], areaLightsources[78]);
		emittance = float(areaLightsources[79]);
	}
	else if(index == 10){
		position = vec3(areaLightsources[80], areaLightsources[81], areaLightsources[82]);
		radius = float(areaLightsources[83]);
		color = vec3(areaLightsources[84], areaLightsources[85], areaLightsources[86]);
		emittance = float(areaLightsources[87]);
	}
	else if(index == 11){
		position = vec3(areaLightsources[88], areaLightsources[89], areaLightsources[90]);
		radius = float(areaLightsources[91]);
		color = vec3(areaLightsources[92], areaLightsources[93], areaLightsources[94]);
		emittance = float(areaLightsources[95]);
	}
	else if(index == 12){
		position = vec3(areaLightsources[96], areaLightsources[97], areaLightsources[98]);
		radius = float(areaLightsources[99]);
		color = vec3(areaLightsources[100], areaLightsources[101], areaLightsources[102]);
		emittance = float(areaLightsources[103]);
	}
	else if(index == 13){
		position = vec3(areaLightsources[104], areaLightsources[105], areaLightsources[106]);
		radius = float(areaLightsources[107]);
		color = vec3(areaLightsources[108], areaLightsources[109], areaLightsources[110]);
		emittance = float(areaLightsources[111]);
	}
	else if(index == 14){
		position = vec3(areaLightsources[112], areaLightsources[113], areaLightsources[114]);
		radius = float(areaLightsources[115]);
		color = vec3(areaLightsources[116], areaLightsources[117], areaLightsources[118]);
		emittance = float(areaLightsources[119]);
	}
	else if(index == 15){
		position = vec3(areaLightsources[120], areaLightsources[121], areaLightsources[122]);
		radius = float(areaLightsources[123]);
		color = vec3(areaLightsources[124], areaLightsources[125], areaLightsources[126]);
		emittance = float(areaLightsources[127]);
	}
	else if(index == 16){
		position = vec3(areaLightsources[128], areaLightsources[129], areaLightsources[130]);
		radius = float(areaLightsources[131]);
		color = vec3(areaLightsources[132], areaLightsources[133], areaLightsources[134]);
		emittance = float(areaLightsources[135]);
	}
	else if(index == 17){
		position = vec3(areaLightsources[136], areaLightsources[137], areaLightsources[138]);
		radius = float(areaLightsources[139]);
		color = vec3(areaLightsources[140], areaLightsources[141], areaLightsources[142]);
		emittance = float(areaLightsources[143]);
	}
	else if(index == 18){
		position = vec3(areaLightsources[144], areaLightsources[145], areaLightsources[146]);
		radius = float(areaLightsources[147]);
		color = vec3(areaLightsources[148], areaLightsources[149], areaLightsources[150]);
		emittance = float(areaLightsources[151]);
	}
	else if(index == 19){
		position = vec3(areaLightsources[152], areaLightsources[153], areaLightsources[154]);
		radius = float(areaLightsources[155]);
		color = vec3(areaLightsources[156], areaLightsources[157], areaLightsources[158]);
		emittance = float(areaLightsources[159]);
	}
}


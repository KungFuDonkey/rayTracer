#version 430
layout(local_size_x = 1, local_size_y = 1) in;
layout(rgba32f, binding = 0) uniform image2D img_output;
uniform vec3 circles[];
uniform vec2 boxes[];
uniform vec2 lightsources[];
uniform vec3 colors[];

ivec2 dims;
void hitLine(vec2 ray_o, vec2 ray_d, vec2 point1, vec2 point2, float tmax, inout bool hit);
bool hitObjects(vec2 ray_o, vec2 ray_d, float tmax);
void lightsource(vec2 ray_o, inout vec4 pixel);

void main(){
	vec4 pixel = vec4(0.0, 0.0, 0.0, 1.0);
	ivec2 pixel_coords = ivec2(gl_GlobalInvocationID.xy);

	float max_x = 1;
	float max_y = 1;
	dims = imageSize(img_output);
	float x = (float(pixel_coords.x * 2 - dims.x) / dims.x);
	float y = (float(pixel_coords.y * 2 - dims.y) / dims.x);

	vec2 ray_o = vec2(x* max_x, y * max_y);

	lightsource(ray_o, pixel);
	pixel.x = (-1/ (1 + pixel.x)) + 1;
	pixel.y = (-1/ (1 + pixel.y)) + 1;
	pixel.z = (-1/ (1 + pixel.z)) + 1;
	imageStore(img_output, pixel_coords, pixel);
}
bool hitObjects(vec2 ray_o, vec2 ray_d, float tmax){
    float b;
    float s;
    float discriminant;
    b = 2.0 * dot(ray_o - circles[0].xy, ray_d);
    discriminant = b * b - 4 * (dot(ray_o - circles[0].xy, ray_o - circles[0].xy) - circles[0].z * circles[0].z);
    if(discriminant >= 0){
       s = (-b - sqrt(discriminant)) / 2;
       s = s < 0 ? (-b + sqrt(discriminant)) / 2 : s;
       if(s >= 0 && s < tmax) return true;
    }
    b = 2.0 * dot(ray_o - circles[1].xy, ray_d);
    discriminant = b * b - 4 * (dot(ray_o - circles[1].xy, ray_o - circles[1].xy) - circles[1].z * circles[1].z);
    if(discriminant >= 0){
       s = (-b - sqrt(discriminant)) / 2;
       s = s < 0 ? (-b + sqrt(discriminant)) / 2 : s;
       if(s >= 0 && s < tmax) return true;
    }
    vec2 line;
    line = normalize(boxes[1] - boxes[0]);
    b = distance(boxes[0], boxes[1]);
    s = (ray_o.y * ray_d.x + ray_d.y * boxes[0].x - ray_d.y * ray_o.x - boxes[0].y * ray_d.x)/(ray_d.x * line.y - ray_d.y * line.x);
    if(s < b && s > 0){
       s = (boxes[0].x - ray_o.x + line.x * s) / ray_d.x;
       if(s >= 0 && s < tmax) return true;
    }
    line = normalize(boxes[2] - boxes[1]);
    b = distance(boxes[1], boxes[2]);
    s = (ray_o.y * ray_d.x + ray_d.y * boxes[1].x - ray_d.y * ray_o.x - boxes[1].y * ray_d.x)/(ray_d.x * line.y - ray_d.y * line.x);
    if(s < b && s > 0){
       s = (boxes[1].x - ray_o.x + line.x * s) / ray_d.x;
       if(s >= 0 && s < tmax) return true;
    }
    line = normalize(boxes[3] - boxes[2]);
    b = distance(boxes[2], boxes[3]);
    s = (ray_o.y * ray_d.x + ray_d.y * boxes[2].x - ray_d.y * ray_o.x - boxes[2].y * ray_d.x)/(ray_d.x * line.y - ray_d.y * line.x);
    if(s < b && s > 0){
       s = (boxes[2].x - ray_o.x + line.x * s) / ray_d.x;
       if(s >= 0 && s < tmax) return true;
    }
    line = normalize(boxes[0] - boxes[3]);
    b = distance(boxes[3], boxes[0]);
    s = (ray_o.y * ray_d.x + ray_d.y * boxes[3].x - ray_d.y * ray_o.x - boxes[3].y * ray_d.x)/(ray_d.x * line.y - ray_d.y * line.x);
    if(s < b && s > 0){
       s = (boxes[3].x - ray_o.x + line.x * s) / ray_d.x;
       if(s >= 0 && s < tmax) return true;
    }
    return false;
}
void lightsource(vec2 ray_o, inout vec4 pixel){
    vec2 lightray;
    float luminance;
    float tmax;
    lightray = lightsources[0] - ray_o ;
    luminance = 1 / (length(lightray) * length(lightray));
    tmax = length(lightray) - 0.0001;
    lightray = normalize(lightray);
    if(!hitObjects(ray_o, lightray, tmax))
       pixel += vec4(colors[0], 0) * luminance;
    lightray = lightsources[1] - ray_o ;
    luminance = 1 / (length(lightray) * length(lightray));
    tmax = length(lightray) - 0.0001;
    lightray = normalize(lightray);
    if(!hitObjects(ray_o, lightray, tmax))
       pixel += vec4(colors[1], 0) * luminance;
}


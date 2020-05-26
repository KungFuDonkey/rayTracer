#version 430
//tells shader how many pixels it has to edit
layout(local_size_x = 1, local_size_y = 1) in;

//image with dimensions
layout(rgba32f, binding = 0) uniform image2D img_output;
ivec2 dims;

//object arrays
uniform vec3 spheres[];
uniform vec3 areaLightsources[];
uniform vec3 directional_lightsources[];
uniform vec4 planes[];
uniform vec3 vertices[];
uniform vec3 colors[];

//function for calculating collisions for normal rays
void calcObjects(vec3 ray_origin, vec3 ray_direction, inout float t, inout float col, inout float absorption, inout vec3 normal);

//function for calculating collisions for light rays
bool calcObjects(vec3 ray_origin, vec3 ray_direction, float tmax);

//function for area lightsources
void calcAreaLightSources(vec3 ray_origin, float absorption, vec3 normal);
//functions for getting the objects out of the arrays
void getColor(int index, out vec3 color);
void getVertice(int index, out vec3 vertice);

vec3 ray_origin;
vec3 color = vec3(0.0, 0.0, 0.0);
vec3 energy;
//main
void main(){

	//initialize the color for this pixel and get invocationID for the ray

	ivec2 pixel_coords = ivec2(gl_GlobalInvocationID.xy);

	//initialization for the ray
	dims = imageSize(img_output);

	for(int y = 0; y < 2; ++y){
		for(int x = 0; x < 2; ++x){
			ray_origin = vec3((float(pixel_coords.x * 2 + 0.5 * x - dims.x) / dims.x), (float(pixel_coords.y * 2 + 0.5 * y - dims.y) / dims.x), 1);
			vec3 ray_direction = ray_origin;
			ray_direction = normalize(ray_direction);
			float t = 100000;
			vec3 next_color = vec3(0, 0, 0);
			vec3 normal = vec3(0, 0, 0);
			float absorption = 0;
			energy = vec3(1, 1, 1);
			float col = 19;
			for(int i = 0; i < 5; ++i){
				calcObjects(ray_origin, ray_direction, t, col, absorption, normal);

				if(t != 100000){

					//update the ray energy, if energy == 0 then stop the loop as there will be no colors left to write 
					getColor(int(col), next_color);
					energy *= next_color;
					if(energy == vec3(0,0,0)) break;

					//create point of intersection 
					ray_origin = ray_origin + t * ray_direction;

					//arealightsources, gets arealightsource and calculates the luminance
					calcAreaLightSources(ray_origin, absorption, normal);

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
		}
	}
	//make sure the colors are within the rgb range
	color.x = (-1 / (1 + color.x)) + 1;
	color.y = (-1 / (1 + color.y)) + 1;
	color.z = (-1 / (1 + color.z)) + 1;
	//store the pixel color in the image
	imageStore(img_output, pixel_coords, vec4(color,1));
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

bool calcObjects(vec3 ray_origin, vec3 ray_direction, float tmax){
    float d;
    float discriminant;
    float s;
    vec3 object_position;
    object_position = normalize(cross(vertices[2] - vertices[0], vertices[1] - vertices[0]));
    d = -dot(object_position, vertices[0]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < tmax){
            if(dot(object_position,cross(vertices[2] - vertices[0], ray_origin + s * ray_direction - vertices[0])) >= 0){
                if(dot(object_position,cross(vertices[1] - vertices[2], ray_origin + s * ray_direction - vertices[2])) >= 0){
                    if(dot(object_position,cross(vertices[0] - vertices[1], ray_origin + s * ray_direction - vertices[1])) >= 0){
                        return true;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[2] - vertices[1], vertices[3] - vertices[1]));
    d = -dot(object_position, vertices[1]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < tmax){
            if(dot(object_position,cross(vertices[2] - vertices[1], ray_origin + s * ray_direction - vertices[1])) >= 0){
                if(dot(object_position,cross(vertices[3] - vertices[2], ray_origin + s * ray_direction - vertices[2])) >= 0){
                    if(dot(object_position,cross(vertices[1] - vertices[3], ray_origin + s * ray_direction - vertices[3])) >= 0){
                        return true;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[1] - vertices[0], vertices[4] - vertices[0]));
    d = -dot(object_position, vertices[0]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < tmax){
            if(dot(object_position,cross(vertices[1] - vertices[0], ray_origin + s * ray_direction - vertices[0])) >= 0){
                if(dot(object_position,cross(vertices[4] - vertices[1], ray_origin + s * ray_direction - vertices[1])) >= 0){
                    if(dot(object_position,cross(vertices[0] - vertices[4], ray_origin + s * ray_direction - vertices[4])) >= 0){
                        return true;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[5] - vertices[1], vertices[4] - vertices[1]));
    d = -dot(object_position, vertices[1]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < tmax){
            if(dot(object_position,cross(vertices[5] - vertices[1], ray_origin + s * ray_direction - vertices[1])) >= 0){
                if(dot(object_position,cross(vertices[4] - vertices[5], ray_origin + s * ray_direction - vertices[5])) >= 0){
                    if(dot(object_position,cross(vertices[1] - vertices[4], ray_origin + s * ray_direction - vertices[4])) >= 0){
                        return true;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[7] - vertices[1], vertices[5] - vertices[1]));
    d = -dot(object_position, vertices[1]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < tmax){
            if(dot(object_position,cross(vertices[7] - vertices[1], ray_origin + s * ray_direction - vertices[1])) >= 0){
                if(dot(object_position,cross(vertices[5] - vertices[7], ray_origin + s * ray_direction - vertices[7])) >= 0){
                    if(dot(object_position,cross(vertices[1] - vertices[5], ray_origin + s * ray_direction - vertices[5])) >= 0){
                        return true;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[3] - vertices[1], vertices[7] - vertices[1]));
    d = -dot(object_position, vertices[1]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < tmax){
            if(dot(object_position,cross(vertices[3] - vertices[1], ray_origin + s * ray_direction - vertices[1])) >= 0){
                if(dot(object_position,cross(vertices[7] - vertices[3], ray_origin + s * ray_direction - vertices[3])) >= 0){
                    if(dot(object_position,cross(vertices[1] - vertices[7], ray_origin + s * ray_direction - vertices[7])) >= 0){
                        return true;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[6] - vertices[2], vertices[3] - vertices[2]));
    d = -dot(object_position, vertices[2]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < tmax){
            if(dot(object_position,cross(vertices[6] - vertices[2], ray_origin + s * ray_direction - vertices[2])) >= 0){
                if(dot(object_position,cross(vertices[3] - vertices[6], ray_origin + s * ray_direction - vertices[6])) >= 0){
                    if(dot(object_position,cross(vertices[2] - vertices[3], ray_origin + s * ray_direction - vertices[3])) >= 0){
                        return true;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[7] - vertices[6], vertices[3] - vertices[6]));
    d = -dot(object_position, vertices[6]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < tmax){
            if(dot(object_position,cross(vertices[7] - vertices[6], ray_origin + s * ray_direction - vertices[6])) >= 0){
                if(dot(object_position,cross(vertices[3] - vertices[7], ray_origin + s * ray_direction - vertices[7])) >= 0){
                    if(dot(object_position,cross(vertices[6] - vertices[3], ray_origin + s * ray_direction - vertices[3])) >= 0){
                        return true;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[4] - vertices[0], vertices[6] - vertices[0]));
    d = -dot(object_position, vertices[0]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < tmax){
            if(dot(object_position,cross(vertices[4] - vertices[0], ray_origin + s * ray_direction - vertices[0])) >= 0){
                if(dot(object_position,cross(vertices[6] - vertices[4], ray_origin + s * ray_direction - vertices[4])) >= 0){
                    if(dot(object_position,cross(vertices[0] - vertices[6], ray_origin + s * ray_direction - vertices[6])) >= 0){
                        return true;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[6] - vertices[0], vertices[2] - vertices[0]));
    d = -dot(object_position, vertices[0]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < tmax){
            if(dot(object_position,cross(vertices[6] - vertices[0], ray_origin + s * ray_direction - vertices[0])) >= 0){
                if(dot(object_position,cross(vertices[2] - vertices[6], ray_origin + s * ray_direction - vertices[6])) >= 0){
                    if(dot(object_position,cross(vertices[0] - vertices[2], ray_origin + s * ray_direction - vertices[2])) >= 0){
                        return true;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[5] - vertices[4], vertices[6] - vertices[4]));
    d = -dot(object_position, vertices[4]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < tmax){
            if(dot(object_position,cross(vertices[5] - vertices[4], ray_origin + s * ray_direction - vertices[4])) >= 0){
                if(dot(object_position,cross(vertices[6] - vertices[5], ray_origin + s * ray_direction - vertices[5])) >= 0){
                    if(dot(object_position,cross(vertices[4] - vertices[6], ray_origin + s * ray_direction - vertices[6])) >= 0){
                        return true;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[5] - vertices[6], vertices[7] - vertices[6]));
    d = -dot(object_position, vertices[6]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < tmax){
            if(dot(object_position,cross(vertices[5] - vertices[6], ray_origin + s * ray_direction - vertices[6])) >= 0){
                if(dot(object_position,cross(vertices[7] - vertices[5], ray_origin + s * ray_direction - vertices[5])) >= 0){
                    if(dot(object_position,cross(vertices[6] - vertices[7], ray_origin + s * ray_direction - vertices[7])) >= 0){
                        return true;
                    }
                }
            }
        }
    }
    return false;
}

void calcObjects(vec3 ray_origin, vec3 ray_direction, inout float t, inout float col, inout float absorption, inout vec3 normal){
    float d;
    float discriminant;
    float s;
    if(dot(ray_direction, planes[0].xyz) > 0){
        s = -(dot(ray_origin, planes[0].xyz) + planes[0].w) / dot(ray_direction, planes[0].xyz);
        if(s > 0 && s < t){
            t = s;
            col = 0;
            normal = -planes[0].xyz;
            absorption = 0,7;
        }
    }
    if(dot(ray_direction, planes[1].xyz) > 0){
        s = -(dot(ray_origin, planes[1].xyz) + planes[1].w) / dot(ray_direction, planes[1].xyz);
        if(s > 0 && s < t){
            t = s;
            col = 0;
            normal = -planes[1].xyz;
            absorption = 0,7;
        }
    }
    if(dot(ray_direction, planes[2].xyz) > 0){
        s = -(dot(ray_origin, planes[2].xyz) + planes[2].w) / dot(ray_direction, planes[2].xyz);
        if(s > 0 && s < t){
            t = s;
            col = 0;
            normal = -planes[2].xyz;
            absorption = 0,7;
        }
    }
    vec3 object_position;
    object_position = normalize(cross(vertices[2] - vertices[0], vertices[1] - vertices[0]));
    d = -dot(object_position, vertices[0]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < t){
            if(dot(object_position,cross(vertices[2] - vertices[0], ray_origin + s * ray_direction - vertices[0])) >= 0){
                if(dot(object_position,cross(vertices[1] - vertices[2], ray_origin + s * ray_direction - vertices[2])) >= 0){
                    if(dot(object_position,cross(vertices[0] - vertices[1], ray_origin + s * ray_direction - vertices[1])) >= 0){
                        t = s;
                        col = 3;
                        normal = -object_position;
                        absorption = 1;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[2] - vertices[1], vertices[3] - vertices[1]));
    d = -dot(object_position, vertices[1]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < t){
            if(dot(object_position,cross(vertices[2] - vertices[1], ray_origin + s * ray_direction - vertices[1])) >= 0){
                if(dot(object_position,cross(vertices[3] - vertices[2], ray_origin + s * ray_direction - vertices[2])) >= 0){
                    if(dot(object_position,cross(vertices[1] - vertices[3], ray_origin + s * ray_direction - vertices[3])) >= 0){
                        t = s;
                        col = 3;
                        normal = -object_position;
                        absorption = 1;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[1] - vertices[0], vertices[4] - vertices[0]));
    d = -dot(object_position, vertices[0]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < t){
            if(dot(object_position,cross(vertices[1] - vertices[0], ray_origin + s * ray_direction - vertices[0])) >= 0){
                if(dot(object_position,cross(vertices[4] - vertices[1], ray_origin + s * ray_direction - vertices[1])) >= 0){
                    if(dot(object_position,cross(vertices[0] - vertices[4], ray_origin + s * ray_direction - vertices[4])) >= 0){
                        t = s;
                        col = 3;
                        normal = -object_position;
                        absorption = 1;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[5] - vertices[1], vertices[4] - vertices[1]));
    d = -dot(object_position, vertices[1]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < t){
            if(dot(object_position,cross(vertices[5] - vertices[1], ray_origin + s * ray_direction - vertices[1])) >= 0){
                if(dot(object_position,cross(vertices[4] - vertices[5], ray_origin + s * ray_direction - vertices[5])) >= 0){
                    if(dot(object_position,cross(vertices[1] - vertices[4], ray_origin + s * ray_direction - vertices[4])) >= 0){
                        t = s;
                        col = 3;
                        normal = -object_position;
                        absorption = 1;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[7] - vertices[1], vertices[5] - vertices[1]));
    d = -dot(object_position, vertices[1]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < t){
            if(dot(object_position,cross(vertices[7] - vertices[1], ray_origin + s * ray_direction - vertices[1])) >= 0){
                if(dot(object_position,cross(vertices[5] - vertices[7], ray_origin + s * ray_direction - vertices[7])) >= 0){
                    if(dot(object_position,cross(vertices[1] - vertices[5], ray_origin + s * ray_direction - vertices[5])) >= 0){
                        t = s;
                        col = 3;
                        normal = -object_position;
                        absorption = 1;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[3] - vertices[1], vertices[7] - vertices[1]));
    d = -dot(object_position, vertices[1]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < t){
            if(dot(object_position,cross(vertices[3] - vertices[1], ray_origin + s * ray_direction - vertices[1])) >= 0){
                if(dot(object_position,cross(vertices[7] - vertices[3], ray_origin + s * ray_direction - vertices[3])) >= 0){
                    if(dot(object_position,cross(vertices[1] - vertices[7], ray_origin + s * ray_direction - vertices[7])) >= 0){
                        t = s;
                        col = 3;
                        normal = -object_position;
                        absorption = 1;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[6] - vertices[2], vertices[3] - vertices[2]));
    d = -dot(object_position, vertices[2]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < t){
            if(dot(object_position,cross(vertices[6] - vertices[2], ray_origin + s * ray_direction - vertices[2])) >= 0){
                if(dot(object_position,cross(vertices[3] - vertices[6], ray_origin + s * ray_direction - vertices[6])) >= 0){
                    if(dot(object_position,cross(vertices[2] - vertices[3], ray_origin + s * ray_direction - vertices[3])) >= 0){
                        t = s;
                        col = 3;
                        normal = -object_position;
                        absorption = 1;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[7] - vertices[6], vertices[3] - vertices[6]));
    d = -dot(object_position, vertices[6]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < t){
            if(dot(object_position,cross(vertices[7] - vertices[6], ray_origin + s * ray_direction - vertices[6])) >= 0){
                if(dot(object_position,cross(vertices[3] - vertices[7], ray_origin + s * ray_direction - vertices[7])) >= 0){
                    if(dot(object_position,cross(vertices[6] - vertices[3], ray_origin + s * ray_direction - vertices[3])) >= 0){
                        t = s;
                        col = 3;
                        normal = -object_position;
                        absorption = 1;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[4] - vertices[0], vertices[6] - vertices[0]));
    d = -dot(object_position, vertices[0]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < t){
            if(dot(object_position,cross(vertices[4] - vertices[0], ray_origin + s * ray_direction - vertices[0])) >= 0){
                if(dot(object_position,cross(vertices[6] - vertices[4], ray_origin + s * ray_direction - vertices[4])) >= 0){
                    if(dot(object_position,cross(vertices[0] - vertices[6], ray_origin + s * ray_direction - vertices[6])) >= 0){
                        t = s;
                        col = 3;
                        normal = -object_position;
                        absorption = 1;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[6] - vertices[0], vertices[2] - vertices[0]));
    d = -dot(object_position, vertices[0]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < t){
            if(dot(object_position,cross(vertices[6] - vertices[0], ray_origin + s * ray_direction - vertices[0])) >= 0){
                if(dot(object_position,cross(vertices[2] - vertices[6], ray_origin + s * ray_direction - vertices[6])) >= 0){
                    if(dot(object_position,cross(vertices[0] - vertices[2], ray_origin + s * ray_direction - vertices[2])) >= 0){
                        t = s;
                        col = 3;
                        normal = -object_position;
                        absorption = 1;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[5] - vertices[4], vertices[6] - vertices[4]));
    d = -dot(object_position, vertices[4]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < t){
            if(dot(object_position,cross(vertices[5] - vertices[4], ray_origin + s * ray_direction - vertices[4])) >= 0){
                if(dot(object_position,cross(vertices[6] - vertices[5], ray_origin + s * ray_direction - vertices[5])) >= 0){
                    if(dot(object_position,cross(vertices[4] - vertices[6], ray_origin + s * ray_direction - vertices[6])) >= 0){
                        t = s;
                        col = 3;
                        normal = -object_position;
                        absorption = 1;
                    }
                }
            }
        }
    }
    object_position = normalize(cross(vertices[5] - vertices[6], vertices[7] - vertices[6]));
    d = -dot(object_position, vertices[6]);
    if(dot(ray_direction, object_position) > 0){
        s = -(dot(ray_origin, object_position) + d) / dot(ray_direction, object_position);
        if(s > 0 && s < t){
            if(dot(object_position,cross(vertices[5] - vertices[6], ray_origin + s * ray_direction - vertices[6])) >= 0){
                if(dot(object_position,cross(vertices[7] - vertices[5], ray_origin + s * ray_direction - vertices[5])) >= 0){
                    if(dot(object_position,cross(vertices[6] - vertices[7], ray_origin + s * ray_direction - vertices[7])) >= 0){
                        t = s;
                        col = 3;
                        normal = -object_position;
                        absorption = 1;
                    }
                }
            }
        }
    }
    d = 2.0 * dot(ray_origin - areaLightsources[0], ray_direction);
    discriminant = d * d - 4 * (dot(ray_origin - areaLightsources[0], ray_origin - areaLightsources[0]) - 0,04);
    if(discriminant >= 0) {
        s = (-d - sqrt(discriminant)) / 2;
        s = s < 0 ? (-d + sqrt(discriminant)) / 2 : s;
        if(s > 0 && s < t) {
            t = s;
            col = 0;
            normal = vec3(0,0,0);
            absorption = 1;
        }
    }
}

void calcAreaLightSources(vec3 ray_origin, float absorption, vec3 normal){
    float lightsource_emittance;
    vec3 light_direction;
    float tmax;
    float angle;
    bool collision;
    vec3 lightsource_color;
    vec3 object_color;
    vec3 point_of_intersection;
    light_direction = areaLightsources[0] - ray_origin;
    tmax = length(light_direction) - 0.0002;
    lightsource_emittance = 10 / (12.456 * length(light_direction) * length(light_direction));
    light_direction = normalize(light_direction);
    if(normal == vec3(0,0,0))
        angle = 1;
    else
        angle = dot(normal, light_direction);
    point_of_intersection = ray_origin + light_direction * 0.0001;
    collision = false;
    if(angle < 0) collision = true;
    if(!collision){
        collision = calcObjects(point_of_intersection, light_direction, tmax);
        if(!collision){
            getColor(int(0), lightsource_color);
            color += lightsource_color * lightsource_emittance * energy * angle * absorption * 0.25;
        }
    }
    tmax = 10000;
    angle = dot(normal, directional_lightsources[0]);
    point_of_intersection = ray_origin + directional_lightsources[0] * 0.0001;
    collision = false;
    if(angle < 0) collision = true;
    if(!collision){
        collision = calcObjects(point_of_intersection, directional_lightsources[0], tmax);
        if(!collision){
            getColor(int(1), lightsource_color);
            color += lightsource_color * energy * 10 * angle * absorption;
        }
    }
}


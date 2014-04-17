require 'rubygems'
require 'chunky_png'

include ChunkyPNG

input = ARGV[0]
output = ARGV[1]

def if_premultiplied(image)
	(0..(image.height - 1)).each do |y|
		(0..(image.width - 1)).each do |x|
		end
	end
end

image = Image.from_file(input)

(0..(image.height - 1)).each do |y|
	(0..(image.width - 1)).each do |x|
		c = image[x, y]
		af = Color.a(c) / 255.0
		nc = Color.rgba(
			(Color.r(c) * af).to_i,
			(Color.g(c) * af).to_i,
			(Color.b(c) * af).to_i,
			Color.a(c))

		image[x, y] = nc
	end
end

image.save(output);
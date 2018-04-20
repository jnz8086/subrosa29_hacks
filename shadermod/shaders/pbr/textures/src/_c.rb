
Size = 2048

def resize(name)
	p "resizing #{name}..."
	system "convert #{name} -resize #{Size}x#{Size} ../#{name}"
end

def combine_pbr(src_name, tmp_name, dst_name = nil)
  dst_name = src_name if dst_name.nil?
    ["DSP", "RGH", "MTL", "AO", "NRM"].each{|t|
		system "convert #{src_name}_#{t}.png -resize #{Size}x#{Size} tmp_#{tmp_name}_#{t}.png -gravity center -composite #{ t=="NRM" ? "../" : "./0tmp_" }#{src_name}_#{tmp_name}_#{t}.png"
	}
	build_pbr("0tmp_#{src_name}_#{tmp_name}", "#{src_name}_#{tmp_name}")
end

def build_pbr(src_name,dst_name = nil)
  p "building #{src_name}..."
  dst_name = src_name if dst_name.nil?
	system "convert #{src_name}_DSP.png #{src_name}_RGH.png #{src_name}_MTL.png #{src_name}_AO.png -resize #{Size}x#{Size} -channel-fx \"gray=>red | gray=>green | gray=>blue | gray=>alpha\" ../#{dst_name}_pbr.png"
end

#resize("BorderedConcreteTiles_NRM.png")
#build_pbr("BorderedConcreteTiles")
#resize("Ash_NRM.png")
#build_pbr("Ash")
#build_pbr("bc_brick01")
#combine_pbr("bc_brick01","windows00")
#combine_pbr("bc_brick01","windows01")
#combine_pbr("bc_brick01","windows02")
#resize("Copper_NRM.png")
#build_pbr("Copper")
#resize("Iron_NRM.png")
#build_pbr("Iron")
#resize("ForestDirt_NRM.png")
#build_pbr("ForestDirt")
#resize("GoldenHerringboneTiles_NRM.png")
#build_pbr("GoldenHerringboneTiles")
#build_pbr("bc_brick04")
#resize("TallBricks_AO_NRM.png")
#build_pbr("TallBricks")
#build_pbr("Oak")
#resize("OxidizedCopper_NRM.png")
#build_pbr("OxidizedCopper")
#resize("OrangeGranite_NRM.png")
#build_pbr("OrangeGranite")
resize("RedPaintedMetal_NRM.png")
build_pbr("RedPaintedMetal")

p "Done!"
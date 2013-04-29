
versions = []

Dir.entries('.').select { |entry| 
  afile = File.join(entry, 'AssemblyInfo.cs')
  if File.file? afile and !(entry =='.' || entry == '..') 
		File.open(afile).read.each_line do |li|
		  match = li.scan(/\[assembly\:\sAssemblyVersion\("(\d+\.\d+\.\d+).\*"\)\]/)

			if match.length > 0 
				#puts match 
				versions << match 
		  end
		end
  end
}

version = versions.uniq

if version.length > 1 
  puts "ERROR: Multiple versions detected"
	exit
end

puts version
    
old_version = version[0][0][0]

puts "Version detected: #{old_version}"

version = old_version.scan(/(\d+)\.(\d+)\.(\d+)/)

new_build = version[0][2].to_i + 1

new_version = "#{version[0][0]}.#{version[0][1]}.#{new_build}"

puts "New version: #{new_version}"
print "Hit Enter to accept, input new version, type exit to leave > "

version_correct = false

while !version_correct do
	response = gets

	if response == "\n"
		version_correct = true
	else
		if response.match(/^(\d+)\.(\d+)\.(\d+)$/)
			new_version = response.delete("\n")
		  version_correct = true
		elsif response == "exit\n"
		  puts "Terminating..."
			exit
		else
		  print "Version number invalid, try again > "
		end
	end
end

puts "Applying version #{new_version}"

Dir.entries('.').select { |entry| 
  afile = File.join(entry, 'AssemblyInfo.cs')
  if File.file? afile and !(entry =='.' || entry == '..') 
		text = File.open(afile).read

		new_text = text.gsub(/\[assembly\:\sAssemblyVersion\("#{old_version}.\*"\)\]/, "[assembly: AssemblyVersion(\"#{new_version}.*\")]")

		if new_text != text
		  File.open(afile, "w") { |file| file.puts new_text }		
	    puts "#{entry} -> #{new_version}"
		end
  end
}

puts 
puts "Press Enter to exit"
gets

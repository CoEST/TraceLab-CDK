package VSM;

import java.util.ArrayList;

public class SourceCodeEntry {
	private String file_name;
	private ArrayList<CodeHashEntry> commit_hash;

	public SourceCodeEntry(String file){
		this.file_name = file;
		this.commit_hash = new ArrayList<CodeHashEntry>();
	}
	
	public String getFileName() {
		return this.file_name;
	}
	
	public void addHash(CodeHashEntry c) {
		this.commit_hash.add(c);
	}
	
	public CodeHashEntry getHashEntry(int i) {
		return this.commit_hash.get(i);
	}
	
	public ArrayList<CodeHashEntry> getAllHashes(){
		return this.commit_hash;
	}
}

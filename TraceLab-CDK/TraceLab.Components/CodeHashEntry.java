package VSM;

import java.util.ArrayList;

public class CodeHashEntry {
	private String hash;
	private String message;
	private ArrayList<String> file_path;
	
	public CodeHashEntry(String s) {
		this.hash = s;
		this.message = null;
		this.file_path = new ArrayList<String>();
	}
	
	public void addMessage(String s) {
		this.message = s;
	}
	
	public void addFilePath(String s) {
		this.file_path.add(s);
	}
	
	public String getHash() {
		return this.hash;
	}
	
	public String getMessage() {
		return this.message;
	}
	
	public String getFilePath(int i) {
		return this.file_path.get(i);
	}
	
	public ArrayList<String> getAllPaths(){
		return this.file_path;
	}
}

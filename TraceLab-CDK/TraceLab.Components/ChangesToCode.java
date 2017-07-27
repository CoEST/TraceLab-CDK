package VSM;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.util.ArrayList;
import java.util.Iterator;

import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;

public class ChangesToCode {
	private ArrayList<SourceCodeEntry> Source;
	private String message;
	private String fp;
	private ArrayList<String> file_path;
	private int sourceIndex;
	private int hashIndex;
	
	public ChangesToCode(File f, ArrayList<SourceCodeEntry> s) throws FileNotFoundException {
		this.Source = s;
		this.ParseChangesCode(f.getAbsolutePath());
	}
	
	public void ParseChangesCode (String filename) throws FileNotFoundException {
		 FileReader reader = new FileReader(new File(filename));

		 JSONParser jsonParser = new JSONParser();	 
		 try {
			 Object obj = jsonParser.parse(reader);
			 JSONObject jsonObject = (JSONObject) obj;
			 for(Iterator iterator = jsonObject.keySet().iterator(); iterator.hasNext();) {
				 String hash = (String) iterator.next();
				 this.findEntryIndex(hash, Source);
				 JSONObject jObject = (JSONObject) jsonObject.get(hash);
				 message = (String) jObject.get("message");
				 Source.get(sourceIndex).getHashEntry(hashIndex).addMessage(message);
				 JSONArray jsonArray = (JSONArray) jObject.get("file_path");
				 for(int i=0; i<jsonArray.size(); i++) {
					 fp = (String) jsonArray.get(i);
					 Source.get(sourceIndex).getHashEntry(hashIndex).addFilePath(fp);
				 }
			 }
		 } catch (Exception e) {
			 e.printStackTrace();
		 }
	}
	
	public ArrayList<SourceCodeEntry> getSource(){
		return this.Source;
	}
	
	private void findEntryIndex(String hash, ArrayList<SourceCodeEntry> Source) {
		int count = 0;
		for(int i = 0; i<Source.size(); i++) {
			ArrayList<CodeHashEntry> commit_hash = Source.get(i).getAllHashes();
			for(int j = 0; j<commit_hash.size(); j++) {
				if(Source.get(i).getHashEntry(j).getHash().equals(hash)) {
					sourceIndex = i;
					hashIndex = j;
				}
			}
		}
	}
}

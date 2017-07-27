package VSM;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.util.ArrayList;

import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;

public class Links {
	private ArrayList<LinkEntry> VSMLinks;
	private String target;
	private String source;
	
	public Links(File f) throws FileNotFoundException {
		this.VSMLinks = new ArrayList<LinkEntry>();
		this.ParseLinks(f.getAbsolutePath());
	}
	
	public ArrayList<LinkEntry> getLinks() {
		return VSMLinks;
	}
	
	public void ParseLinks (String filename) throws FileNotFoundException {
		 FileReader reader = new FileReader(new File(filename));
		 
		 JSONParser jsonParser = new JSONParser();	 
		 try {
			 Object obj = jsonParser.parse(reader);
			 JSONArray jsonArray = (JSONArray) obj;
			 for(int i=0; i<jsonArray.size(); i++) {
				 JSONObject jsonObject = (JSONObject) jsonArray.get(i);
				 LinkEntry e = new LinkEntry();
				 target = (String) jsonObject.get("target_issue_id");
				 source = (String) jsonObject.get("source_issue_id");
				 e.setVariables(target, source);
				 e.setBoolean();
				 VSMLinks.add(e);
			 }
		 } catch (Exception e) {
			 e.printStackTrace();
		 }
	}
}

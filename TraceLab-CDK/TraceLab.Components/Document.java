package VSM;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;

import gnu.trove.map.hash.THashMap;

import java.util.ArrayList;
import java.util.Set;

import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;

/**
 * This class represents one document.
 * It will keep track of the term frequencies.
 * @author swapneel
 *
 */
public class Document implements Comparable<Document>{
	public boolean stopwords_selected;
	public boolean stemmer_selected;
	
	/**
	 * A hashmap for term frequencies.
	 * Maps a term to the number of times this terms appears in this document. 
	 */
	private THashMap<String, Integer> termFrequency;
	
	/**
	 * The name of the file to read.
	 */
	private String filename;
	
	//ArrayList containing the information necessary to connect each filename with its source code files
	private ArrayList<SourceCodeEntry> Source;
	
	/**
	 * The constructor.
	 * It takes in the name of a file to read.
	 * It will read the file and pre-process it.
	 * @param filename the name of the file
	 */
	public Document(String filename) {
		this.filename = filename;
		termFrequency = new THashMap<String, Integer>();
		
		readFileAndPreProcess();
	}
	
	// Constructor used if source code is included in analysis attempt
	public Document(String filename, ArrayList<SourceCodeEntry> Source, String codeDirectory) {
		this.filename = filename;
		termFrequency = new THashMap<String, Integer>();
		
		readFileAndPreProcess(Source, codeDirectory);
	}
	
	/**
	 * This method will read in the file and do some pre-processing.
	 * The following things are done in pre-processing:
	 * Every word is converted to lower case.
	 * Every character that is not a letter or a digit is removed.
	 */
	private void readFileAndPreProcess() {
		try {
			String output = this.ParseJSON(this.filename);
			Stopwords sw = new Stopwords();
			Stemmer s = new Stemmer();
			
			for (String nextWord : output.split("\\s+")){
				String filteredWord = nextWord.replaceAll("[^A-Za-z0-9]", "").toLowerCase();
				if(stopwords_selected = true) {
					filteredWord = sw.removeSingle(filteredWord);
				}
				if(stemmer_selected = true) {
					s.add(filteredWord.toCharArray(), filteredWord.length());
					s.stem();
					filteredWord = s.toString();
				}
				
				if (!(filteredWord.equals(""))) {
					if (termFrequency.containsKey(filteredWord)) {
						int oldCount = termFrequency.get(filteredWord);
						termFrequency.put(filteredWord, ++oldCount);
					} else {
						termFrequency.put(filteredWord, 1);
					}
				}
			}
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		}
	}
	
	private void readFileAndPreProcess(ArrayList<SourceCodeEntry> Source, String codeDirectory) {
		try {
			StringBuilder temp = new StringBuilder();
			temp.append(this.ParseJSON(this.filename));
			temp.append(getSourceCode(Source, codeDirectory));
			String output = temp.toString();
			Stopwords sw = new Stopwords();
			Stemmer s = new Stemmer();
			
			for (String nextWord : output.split("\\s+")){
				String filteredWord = nextWord.replaceAll("[^A-Za-z0-9]", "").toLowerCase();
				if(stopwords_selected = true) {
					filteredWord = sw.removeSingle(filteredWord);
				}
				if(stemmer_selected = true) {
					s.add(filteredWord.toCharArray(), filteredWord.length());
					s.stem();
					filteredWord = s.toString();
				}
				
				if (!(filteredWord.equals(""))) {
					if (termFrequency.containsKey(filteredWord)) {
						int oldCount = termFrequency.get(filteredWord);
						termFrequency.put(filteredWord, ++oldCount);
					} else {
						termFrequency.put(filteredWord, 1);
					}
				}
			}
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		}
	}
	
	/**
	 * This method will return the term frequency for a given word.
	 * If this document doesn't contain the word, it will return 0
	 * @param word The word to look for
	 * @return the term frequency for this word in this document
	 */
	public double getTermFrequency(String word) {
		if (termFrequency.containsKey(word)) {
			return termFrequency.get(word);
		} else {
			return 0;
		}
	}
	
	/**
	 * This method will return a set of all the terms which occur in this document.
	 * @return a set of all terms in this document
	 */
	public Set<String> getTermList() {
		return termFrequency.keySet();
	}

	@Override
	/**
	 * The overriden method from the Comparable interface.
	 */
	public int compareTo(Document other) {
		return filename.compareTo(other.getFileName());
	}
	
	/**
	 * @return the filename
	 */
	public String getFileName() {
		return filename;
	}
	
	public String ParseJSON (String filename) throws FileNotFoundException {
		 FileReader reader = new FileReader(new File(filename));
		 
		 JSONParser jsonParser = new JSONParser();	 
		 try {
			 Object obj = jsonParser.parse(reader);
			 JSONObject jsonObject = (JSONObject) obj;
			 StringBuilder combineJSON = new StringBuilder();
			 combineJSON.append((String) jsonObject.get("id")).append('\n');
			 combineJSON.append((String) jsonObject.get("priority")).append('\n');
			 combineJSON.append((String) jsonObject.get("summary")).append('\n');
			 combineJSON.append((String) jsonObject.get("type")).append('\n');
			 combineJSON.append((String) jsonObject.get("description")).append('\n');
			 combineJSON.append((String) jsonObject.get("status")).append('\n');
			 combineJSON.append((String) jsonObject.get("resolution"));
			 String JSONString = combineJSON.toString();
			 return JSONString;
		 } catch (Exception e) {
			 e.printStackTrace();
		 }
		 String error = "JSON parser not working";
		 return error;
		}
	
	/*
	 * Will remove all parts of the file path excluding the base file name
	 */
	public String removeExtension() {
		String s = this.getFileName();
		s = s.substring(s.lastIndexOf(File.separator)+1, s.lastIndexOf('.'));
		return s;
	}
	
	public String getSourceCode(ArrayList<SourceCodeEntry> Source, String codeDirectory) {
		System.out.println(Source.size());
		String sourceCode = new String();
		StringBuilder allCode = new StringBuilder();
		String file_name = removeExtension();
		for(int i = 0; i<Source.size(); i++) {
			if(Source.get(i).getFileName().equals(file_name)) {
				System.out.println("Source for "+file_name+" found!");
				System.out.println(Source.get(i).getAllHashes().size());
				for(int j=0; j<Source.get(i).getAllHashes().size(); j++) {
					CodeHashEntry temp = Source.get(i).getHashEntry(j);
					System.out.println(temp.getMessage());
					allCode.append(temp.getMessage()); //append message found in change_set_to_code file
					for(int k=0; k<temp.getAllPaths().size(); k++) {
						try {
							System.out.println(temp.getFilePath(k));
							System.out.println(temp.getHash());
							String fileContent = GitFileGetter.getFileContent(codeDirectory, temp.getFilePath(k), temp.getHash());
							allCode.append(fileContent);
						} catch (IOException e) {
							e.printStackTrace();
						}
					}
				}
			}
		}
		sourceCode = allCode.toString();
		System.out.println(sourceCode);
		return sourceCode;
	}
	
	/*public String checkFilePath(String s) {
		if(System.getProperty("Windows")
		return s;
	}*/
}
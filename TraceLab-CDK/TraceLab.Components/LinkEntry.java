package VSM;

public class LinkEntry {
	private boolean known;
	private String target;
	private String source;
	private double VSMscore;
	
	public LinkEntry() {
		known = false;
		target = null;
		source = null;
		VSMscore = 0;
	}
	
	public void setBoolean() {
		this.known = true;
	}
	
	public void setVariables(String t, String s) {
		this.target = t;
		this.source = s;
	}
	
	public void setScore(double d) {
		this.VSMscore = d;
	}
	
	public boolean getKnownStatus() {
		return this.known;
	}
	
	public String getTarget() {
		return this.target;
	}
	
	public String getSource() {
		return this.source;
	}
	
	public double getScore() {
		return this.VSMscore;
	}
}

package file;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;

/**RC4文件加密，可以对文件进行简单的加密
 * @author liulei
 *
 */
public class RC4 {
	private int S[] = new int[256];
	private int T[] = new int[256];
	private int key[];
	private int bufferSize = 4096;
	private int i;
	private int j;
	public int getBufferSize() {
		return bufferSize;
	}
	public void setBufferSize(int bufferSize) {
		this.bufferSize = bufferSize;
	}
	
	/**初始化
	 * @param key
	 */
	private void init(String key) {
		i = 0;
		j = 0;
		initS();
		initT(key);
		substitution();
	}
	
	/**初始化S
	 * 
	 */
	private void initS() {
		for(int i = 0; i < 256; i++) {
			S[i] = i;
		}
	}
	
	/**初始化T
	 * @param key
	 */
	private void initT(String key) {
		byte[] b = key.getBytes();
		this.key = new int[b.length];
		for(int i = 0; i < b.length; i++) {
			this.key[i] = b[i];
		}
		for(int i = 0, j = 0; i < 256; i++, j++) {
			if(j == this.key.length)
				j = 0;
			T[i] = this.key[j];
		}
	}
	
	/**S置换
	 * 
	 */
	private void substitution() {
		for(int i = 0, j = 0; i < 256; i++) {
			j = (j + S[i]+ T[i]) % 256;
			int temp = S[i];
			S[i] = S[j];
			S[j] = temp;
		}
	}
	
	/**生成一字节密钥
	 * @return
	 */
	private int makeK() {
		i = (i + 1) % 256;
		j = (j + S[i]) % 256;
		int temp = S[i];
		S[i] = S[j];
		S[j] = temp;
		int t = (S[i] + S[j]) % 256;
		return S[t];
	}
	
	/**加解密方法
	 * @param path 文件路径及文件名
	 * @param key 秘钥
	 * @return 返回值0表示运算成功，返回值为-1表示文件不存在，返回值为-2表示出现异常，返回值为-3表示key长度为零
	 */
	public int xor(String path, String key) {
		File file = new File(path);
		if (!file.exists()) {
			return -1;
		}
		
		init(key);
		if (this.key.length == 0) {
			return -3;
		}
		
		long fileSize = file.length();
		long flag = 0;
		try {
			FileInputStream fileInputStream = new FileInputStream(file);
			FileOutputStream fileOutputStream = new FileOutputStream(path + ".rc4");
			byte buffer[] = new byte[bufferSize];
			while(fileInputStream.read(buffer, 0, bufferSize) != -1) {
				if (fileSize-flag < bufferSize) {
					for(int m = 0; flag < fileSize; flag++, m++) {
						int k = makeK();
						fileOutputStream.write(buffer[m]^(byte)k);
					}
					break;
				}
				for(int m = 0; m < bufferSize; m++, flag++) {
					int k = makeK();
					buffer[m] ^= (byte) k;
				}
				fileOutputStream.write(buffer, 0, bufferSize);
			}
			fileInputStream.close();
			fileOutputStream.close();
			file.delete();
			File newFile = new File(path + ".rc4");
			newFile.renameTo(file);
			return 0;
		} catch (FileNotFoundException e) {
			e.printStackTrace();
		} catch (IOException e) {
			e.printStackTrace();
		}
		return -2;
		
	}
}

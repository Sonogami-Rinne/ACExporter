using System.IO;

namespace PmxLib;

public interface IPmxStreamIO
{
	void FromStreamEx(Stream s, PmxElementFormat f);

	void ToStreamEx(Stream s, PmxElementFormat f);
}

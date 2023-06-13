export class Parser {
    public static async freeCommandParse(command: string): Promise<string> {
        //example string: 
        //total used free shared buff / cache available Mem: 1.9Gi 327Mi 611Mi 16Mi 1.0Gi 1.4Gi Swap: 975Mi 89Mi 886Mi 
        const splitString = command.split(": ")
        const result = splitString[1].split('i')
    
        return `${result[1]} / ${result[0]}`
    }

    public static async mpstatCommandParse(data: string): Promise<string> {
        const { idle } = JSON.parse(data).sysstat.hosts[0].statistics[0]['cpu-load'][0];
        return (100 - idle).toFixed(2).toString() + '%';
    }

    public static async dfCommandParse(data: string): Promise<string | undefined> {
        const lines = data.trim().split('\n');

        for (let i = 0; i < lines.length; i++) {
            const columns = lines[i].trim().split(/\s+/);

            const rootDirectoryRegex = /^\/\s*$/
            if (rootDirectoryRegex.test(columns[columns.length - 1])) {
                return `${columns[2]} / ${columns[1]}`
            }
        }
        return "can't find root dir"
    }
}
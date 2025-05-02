/**
 * OrionIRC Server JavaScript API TypeScript Definitions
 * Auto-generated documentation
 **/

// Constants

/**
 * VERSION constant 
 * ""1.0.0""
 */
declare const VERSION: string;


/**
 * JsLoggerModule module
 */
declare const logger: {
    /**
     * Log an informational message
     * @param message string
     * @param args any[]
     */
    info(message: string, args: any[]): void;
    /**
     * Log a warning message
     * @param message string
     * @param args any[]
     */
    warn(message: string, args: any[]): void;
    /**
     * Log an error message
     * @param message string
     * @param args any[]
     */
    error(message: string, args: any[]): void;
    /**
     * Log a critical message
     * @param message string
     * @param args any[]
     */
    critical(message: string, args: any[]): void;
    /**
     * Log a debug message
     * @param message string
     * @param args any[]
     */
    debug(message: string, args: any[]): void;
};


export declare var terminal : Terminal;
export type Terminal = {
    getBlocks(): Promise<Block[]>;
}
export type Block = {
    getDisplayName(): Promise<string>;
    setDisplayName(value: string): Promise<void>;
}
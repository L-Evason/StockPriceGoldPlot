import argparse
import os
import yfinance as yf

def ticker_to_csv(ticker, output_dir='.'):
    data = yf.download(ticker, interval="1mo")
    monthly_open = data['Open']
    os.makedirs(output_dir, exist_ok=True)  # Ensure dir exists
    ticker_file = os.path.join(output_dir, f'{ticker}_monthly_open.csv')
    monthly_open.to_csv(ticker_file)
    print(f"Data for {ticker} saved to {ticker_file}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Fetch monthly open prices for a given stock ticker.")
    parser.add_argument("ticker", help="Stock ticker symbol")
    parser.add_argument("--output-dir", default='.', help="Directory to save the CSV file")
    args = parser.parse_args()
    ticker_to_csv(args.ticker, args.output_dir)
